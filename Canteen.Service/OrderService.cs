using System.Transactions;
using Canteen.Data.Entities;
using Canteen.Data.Repositories.Interfaces;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Category;
using Canteen.Shared.Dtos.Item;
using Canteen.Shared.Dtos.Notification;
using Canteen.Shared.Dtos.Order;
using Canteen.Shared.Dtos.OrderDetails;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Enums;
using Canteen.Shared.Enums.Notification;
using Lib.Authorization.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Canteen.Service;

public class OrderService(
    IUnitOfWork unitOfWork,
    IAuthService authService,
    IOrderRepository orderRepository,
    INotificationService notificationService,
    IUserBalanceService userBalanceService
) : IOrderService
{
    public async Task<OrderDto> GetOrderByIdAsync(int id)
    {
        var order = await orderRepository.GetOrderByIdAsync(id).ConfigureAwait(false);
        if (order is null)
            return null;
        return new OrderDto
        {
            Id = order.Id,
            TotalAmount = order.TotalAmount,
            OrderStatus = order.OrderStatus,
            Remarks = order.Remarks,
            UserId = order.UserId,
            OrderDetails = order
                .OrderDetails.Select(x => new OrderDetailsDto
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    Amount = x.Amount,
                    Item = new ItemDto
                    {
                        Id = x.Item.Id,
                        Name = x.Item.Name,
                        Description = x.Item.Description,
                        Price = x.Item.Price,
                        CategoryId = x.Item.CategoryId,
                        Category = new CategoryDto
                        {
                            CategoryName = x.Item.Category?.CategoryName,
                            Description = x.Item.Category?.Description,
                        }
                    }
                })
                .ToList(),
        };
    }

    public async Task<List<OrderDto>> GetOrderByStatusAsync(OrderStatus orderStatus)
    {
        var orders = await orderRepository.GetOrderByStatusAsync(orderStatus).ConfigureAwait(false);
        return orders
            .Select(order => new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                Remarks = order.Remarks,
                UserId = order.UserId,
            })
            .ToList();
    }

    public async Task<List<OrderDto>> GetOrderByUserIdAsync(string userId)
    {
        var orders = await orderRepository.GetOrderByUserIdAsync(userId).ConfigureAwait(false);
        if (orders is null)
            return null;
        return orders
            .Select(order => new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                Remarks = order.Remarks,
                UserId = order.UserId,
            })
            .ToList();
    }

    public async Task<ResponseDto> ChangeOrderStatusAsync(int orderId, OrderStatus orderStatus)
    {
        try
        {
            using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var order = await orderRepository.GetOrderByIdAsync(orderId);
            order.OrderStatus = orderStatus;
            NotificationDto notification;
            await unitOfWork.Repository<Order>().UpdateAsync(order).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);
            switch (orderStatus)
            {
                case OrderStatus.Approved:
                    notification = new NotificationDto()
                    {
                        Title = "Order Approved",
                        Description = $"Your order has been approved.",
                        Type = NotificationType.Order,
                        Audience = NotificationAudience.Customer
                    };
                    await notificationService.SubmitNotificationAsync(notification);
                    break;
                case OrderStatus.InProgress:
                    notification = new NotificationDto()
                    {
                        Title = "Order Inprogress",
                        Description = $"Your order is inprogress.",
                        Type = NotificationType.Order,
                        Audience = NotificationAudience.Customer
                    };
                    await notificationService.SubmitNotificationAsync(notification);
                    break;
                case OrderStatus.Rejected:
                    notification = new NotificationDto()
                    {
                        Title = "Order Rejected",
                        Description = $"Your order has been Rejected.",
                        Type = NotificationType.Order,
                        Audience = NotificationAudience.Customer
                    };
                    await notificationService.SubmitNotificationAsync(notification);
                    break;
                case OrderStatus.Completed:
                    notification = new NotificationDto()
                    {
                        Title = "Order Completed",
                        Description = $"Your order has been completed.",
                        Type = NotificationType.Order,
                        Audience = NotificationAudience.Customer
                    };
                    await notificationService.SubmitNotificationAsync(notification);
                    break;
                default:
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = $"Invalid orderStatus: {orderStatus}!!!"
                    };
            }
            tx.Complete();
            return new ResponseDto
            {
                IsSuccess = true,
                Message = $"Order Status changed to {orderStatus}!!!"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = $"Error:{ex.Message}"
            };
        }
    }

    public async Task<ResponseDto> SubmitOrderAsync(OrderDto orderDto)
    {
        try
        {
            var totalAmount = orderDto.OrderDetails.Sum(x => x.Amount * x.Quantity);
            var userBalance = await userBalanceService.GetBalanceAsync(orderDto.UserId);
            if (totalAmount > userBalance.Balance)
            {
                return new ResponseDto { IsSuccess = false, Message = "Insufficient fund, please recharge your balance." };
            }
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var newOrder = new Order()
            {
                Id = orderDto.Id,
                TotalAmount = totalAmount,
                OrderStatus = OrderStatus.Submitted,
                OrderDate = DateTime.UtcNow,
                Remarks = orderDto.Remarks,
                UserId = authService.GetCurrentUserId(),
                OrderDetails = orderDto
                    .OrderDetails.Select(x => new OrderDetail
                    {
                        Id = x.Id,
                        Amount = x.Amount,
                        Quantity = x.Quantity,
                        ItemId = x.ItemId,
                    })
                    .ToList(),
            };
            await unitOfWork.Repository<Order>().AddAsync(newOrder).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            await userBalanceService.UpdateBalanceAsync(orderDto.UserId, totalAmount);

            var notification = new NotificationDto()
            {
                Title = "Order submitted",
                Description = $"Order of total amount of ${totalAmount} has been submitted.",
                Type = NotificationType.Order,
                Audience = NotificationAudience.Customer
            };
            await notificationService.SubmitNotificationAsync(notification);
            transaction.Complete();
            return new ResponseDto { Message = "Order submitted", IsSuccess = true, };
        }
        catch (Exception ex)
        {
            return new ResponseDto { Message = $"Error:{ex.Message}", IsSuccess = false };
        }
    }

    public async Task<List<OrderDetailsDto>> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        var orderDetails = await orderRepository.GetOrderDetailsByOrderIdAsync(orderId).ConfigureAwait(false);
        if (orderDetails is null) return null;
        return orderDetails
            .Select(orderDetail => new OrderDetailsDto
            {
                Id = orderDetail.Id,
                ItemId = orderDetail.ItemId,
                Item = new ItemDto
                {
                    Id = orderDetail.Item.Id,
                    Name = orderDetail.Item.Name,
                    Description =orderDetail.Item.Description,
                    Price = orderDetail.Item.Price,
                    CategoryId = orderDetail.Item.CategoryId,
                    Category = new CategoryDto
                    { 
                        CategoryName = orderDetail.Item.Category?.CategoryName,
                        Description = orderDetail.Item.Category?.Description
                    }
                },
                Amount = orderDetail.Amount,
                Quantity = orderDetail.Quantity
            })
            .ToList();
    }
}
