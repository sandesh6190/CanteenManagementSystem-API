using Canteen.Data.Entities;
using Canteen.Data.Repositories.Interfaces;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Canteen.Data.Repositories;

public class OrderRepository(ApplicationDbContext context) : IOrderRepository
{
    public async Task<Order> GetOrderByIdAsync(int id)
    {
        var order = await context
            .Orders.Where(x => x.Id == id)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Item)
            .ThenInclude(x => x.Category)
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);
        return order;
    }

    public async Task<List<Order>> GetOrderByStatusAsync(OrderStatus orderStatus)
    {
        var order = await context
            .Orders.Where(x => x.OrderStatus == orderStatus)
            .ToListAsync()
            .ConfigureAwait(false);
        return order;
    }

    public async Task<List<Order>> GetOrderByUserIdAsync(string userId)
    {
        var order = await context
            .Orders.Where(x => x.UserId == userId)
            .ToListAsync()
            .ConfigureAwait(false);
        return order;
    }
    public async Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        var orderDetails = await context
            .OrderDetails.Where(x => x.OrderId == orderId)
            .Include(x => x.Item)
            .ThenInclude(x => x.Category)
            .ToListAsync()
            .ConfigureAwait(false);
        return orderDetails;
    }
}
