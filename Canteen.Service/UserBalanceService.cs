using Canteen.Data.Entities;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Dtos.UserBalance;
using Lib.Authorization.Entities;
using Lib.Authorization.Interfaces;

namespace Canteen.Service;

public class UserBalanceService(IUnitOfWork unitOfWork, IAuthService authService)
    : IUserBalanceService
{
    public async Task<List<UserBalanceDto>> GetBalancesAsync()
    {
        var balances = await unitOfWork
            .Repository<UserBalance>()
            .GetAllAsync(includeProperties: $"{nameof(UserBalance.User)}")
            .ConfigureAwait(false);

        return balances
            .Select(balance => new UserBalanceDto
            {
                UserId = balance.UserId,
                UserFullName = balance.User.FullName,
                Balance = balance.Balance
            })
            .ToList();
    }

    public async Task<UserBalanceDto> GetBalanceAsync(string userId)
    {
        var balance = await unitOfWork
            .Repository<UserBalance>()
            .GetAsync(x => x.UserId == userId, includeProperties: $"{nameof(UserBalance.User)}")
            .ConfigureAwait(false);

        var user = await unitOfWork
            .Repository<ApplicationUser>()
            .GetAsync(x => x.Id == userId)
            .ConfigureAwait(false);

        if (balance == null && userId != null)
            return new UserBalanceDto { UserFullName = user?.FullName, Balance = 0 };

        return new UserBalanceDto
        {
            UserFullName = balance.User.FullName,
            Balance = balance.Balance
        };
    }

    public async Task<ResponseDto> SubmitBalanceAsync(string redeemNumber)
    {
        var redeem = await unitOfWork
            .Repository<Redeem>()
            .GetAsync(
                x => x.RedeemNo == redeemNumber,
                includeProperties: $"{nameof(Redeem.RedeemType)}"
            )
            .ConfigureAwait(false);

        if (redeem == null)
            return new ResponseDto { Message = "Redeem Number not found", IsSuccess = false };

        if (redeem.IsUsed)
            return new ResponseDto
            {
                Message = "Redeem Number has already been used",
                IsSuccess = false
            };

        var amountChange = redeem.RedeemType.Amount;
        var userId = authService.GetCurrentUserId();

        var userAmount = await unitOfWork
            .Repository<UserBalance>()
            .GetAsync(x => x.UserId == userId)
            .ConfigureAwait(false);

        if (userAmount == null)
        {
            userAmount = new UserBalance { UserId = userId, Balance = amountChange, };
            await unitOfWork.Repository<UserBalance>().AddAsync(userAmount).ConfigureAwait(false);
        }
        else
        {
            userAmount.Balance += amountChange;

            await unitOfWork
                .Repository<UserBalance>()
                .UpdateAsync(userAmount)
                .ConfigureAwait(false);
        }

        redeem.IsUsed = true;

        await unitOfWork.Repository<Redeem>().UpdateAsync(redeem).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return new ResponseDto { Message = "User balance successfully updated", IsSuccess = true };
    }

    public async Task<ResponseDto> UpdateBalanceAsync(string userId, decimal balance)
    {
        var userBalance = await unitOfWork
            .Repository<UserBalance>()
            .GetAsync(x => x.UserId == userId, includeProperties: $"{nameof(UserBalance.User)}")
            .ConfigureAwait(false);
        if (userBalance.Balance < balance)
            return new ResponseDto { Message = "Insufficent fund", IsSuccess = false };
        userBalance.Balance -= balance;
        await unitOfWork.Repository<UserBalance>().UpdateAsync(userBalance).ConfigureAwait(false);
        return new ResponseDto { Message = "Balance updated successfully ", IsSuccess = true };
    }
}
