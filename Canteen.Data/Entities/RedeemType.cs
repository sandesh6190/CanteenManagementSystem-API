namespace Canteen.Data.Entities;

public class RedeemType : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public ICollection<Redeem> Redeems { get; set; }
}