using Canteen.Shared.Dtos.Order;
using Canteen.Shared.Dtos.OrderDetails;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Enums;

namespace Canteen.Service.Interface;

public interface IOrderService
{
    Task<ResponseDto> SubmitOrderAsync(OrderDto orderDto);
    Task<OrderDto> GetOrderByIdAsync(int id);
    Task<List<OrderDto>> GetOrderByUserIdAsync(string userId);
    Task<List<OrderDto>> GetOrderByStatusAsync(OrderStatus orderStatus);
    Task<ResponseDto> ChangeOrderStatusAsync(int orderId, OrderStatus orderStatus);
    Task<List<OrderDetailsDto>> GetOrderDetailsByOrderIdAsync(int orderId);
}