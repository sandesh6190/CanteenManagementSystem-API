using Canteen.Shared.Dtos.RedeemType;
using Canteen.Shared.Dtos.Response;

namespace Canteen.Service.Interface;
public interface IRedeemTypeService
{
    Task<List<RedeemTypeDto>> GetRedeemTypesAsync();
    Task<RedeemTypeDto> GetRedeemTypeAsync(int id);
    Task<ResponseDto> SubmitRedeemTypeAsync(RedeemTypeDto redeemTypeDto);
    Task<ResponseDto> DeleteRedeemTypeAsync(int id);
}