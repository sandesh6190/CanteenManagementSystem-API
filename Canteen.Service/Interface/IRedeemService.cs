using Canteen.Shared.Dtos.Redeem;
using Canteen.Shared.Dtos.Response;

namespace Canteen.Service.Interface;

public interface IRedeemService
{
    Task<List<RedeemDto>> GetRedeemsAsync();
    Task<List<RedeemDto>> GetRedeemsByStatusAsync(bool isused);
    Task<List<RedeemDto>> GetRedeemsByRedeemTypeAsync(int redeemTypeId);
    Task<ResponseDto> SubmitRedeemAsync(RedeemDto redeem);
}