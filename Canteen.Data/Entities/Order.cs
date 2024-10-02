using Canteen.Shared.Enums;
using Lib.Authorization.Entities;

namespace Canteen.Data.Entities;

public class Order
{
    public int Id { get; set; }
    public Decimal TotalAmount { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }
    public string Remarks { get; set; }
    public string UserId { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = [];
}
