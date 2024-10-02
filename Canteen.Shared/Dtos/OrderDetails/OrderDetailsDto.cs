using Canteen.Shared.Dtos.Item;

namespace Canteen.Shared.Dtos.OrderDetails;

public class OrderDetailsDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public ItemDto Item { get; set; }
    public Decimal Amount { get; set; }
    public int Quantity { get; set; }
}
