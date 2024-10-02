namespace Canteen.Data.Entities;

public class Redeem: BaseEntity
{
    public int RedeemTypeId { get; set; }
    public RedeemType RedeemType { get; set; }
    public bool IsUsed { get; set; }
    public string RedeemNo { get; set; }
}