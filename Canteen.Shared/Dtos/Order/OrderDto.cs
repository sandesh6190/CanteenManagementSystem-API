using Canteen.Shared.Dtos.OrderDetails;
using Canteen.Shared.Enums;

namespace Canteen.Shared.Dtos.Order;

public class OrderDto
{
    public int Id { get; set; }
    public Decimal TotalAmount { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime OrderDate { get; set; }
    public string Remarks { get; set; }
    public string UserId { get; set; }
    public List<OrderDetailsDto> OrderDetails { get; set; } = [];
}
