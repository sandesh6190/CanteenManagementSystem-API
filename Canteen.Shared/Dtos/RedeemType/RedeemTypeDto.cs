namespace Canteen.Shared.Dtos.RedeemType;

public class RedeemTypeDto : BaseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
}