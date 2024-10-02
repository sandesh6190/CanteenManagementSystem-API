namespace Canteen.Data.Entities;

public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; }
    public Decimal Amount { get; set; }
    public int Quantity { get; set; }
}
