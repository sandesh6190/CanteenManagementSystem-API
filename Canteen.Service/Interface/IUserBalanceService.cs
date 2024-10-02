using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Dtos.UserBalance;

namespace Canteen.Service.Interface;

public interface IUserBalanceService
{
    Task<List<UserBalanceDto>> GetBalancesAsync();
    Task<UserBalanceDto> GetBalanceAsync(string userId);
    Task<ResponseDto> SubmitBalanceAsync(string redeemNumber);
    Task<ResponseDto> UpdateBalanceAsync(string userId, decimal balance);
}
