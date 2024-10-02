using Canteen.Shared.Dtos.RedeemType;

namespace Canteen.Shared.Dtos.Redeem;

public class RedeemDto : BaseDto
{
    public int RedeemTypeId { get; set; }
    public RedeemTypeDto RedeemType { get; set; }
    public bool IsUsed { get; set; }
    public string RedeemNo { get; set; }
    public int Count { get; set; }
}