using Canteen.Data.Entities;
using Canteen.Shared.Enums;

namespace Canteen.Data.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order> GetOrderByIdAsync(int id);
    Task<List<Order>> GetOrderByUserIdAsync(string userId);
    Task<List<Order>> GetOrderByStatusAsync(OrderStatus orderStatus);
    Task<List<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
}
