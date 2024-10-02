using Canteen.Data.Entities;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Redeem;
using Canteen.Shared.Dtos.RedeemType;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Helpers;
using Lib.Authorization.Interfaces;

namespace Canteen.Service;

public class RedeemService(IUnitOfWork unitOfWork, IAuthService authService) : IRedeemService
{
    public async Task<List<RedeemDto>> GetRedeemsAsync()
    {
        var redeems = await unitOfWork
            .Repository<Redeem>()
            .GetAllAsync(
                includeProperties: $"{nameof(Redeem.RedeemType)},{nameof(Redeem.CreatedBy)},{nameof(Redeem.UpdatedBy)}"
            )
            .ConfigureAwait(false);

        var sortedRedeems = redeems.OrderByDescending(redeem => redeem.CreatedDateTime).ToList();

        return sortedRedeems
            .Select(redeem => new RedeemDto
            {
                Id = redeem.Id,
                RedeemNo = redeem.RedeemNo,
                RedeemTypeId = redeem.RedeemTypeId,
                RedeemType = new RedeemTypeDto
                {
                    Name = redeem.RedeemType?.Name,
                    Amount = redeem.RedeemType.Amount
                },
                CreatedBy = redeem.CreatedBy?.FullName,
                UpdatedBy = redeem.UpdatedBy?.FullName,
                CreatedDateTime = redeem.CreatedDateTime,
                UpdatedDateTime = redeem.UpdatedDateTime
            })
            .ToList();
    }

    public async Task<List<RedeemDto>> GetRedeemsByRedeemTypeAsync(int redeemTypeId)
    {
        var redeems = await unitOfWork
            .Repository<Redeem>()
            .GetAllAsync(
                redeem => redeem.RedeemTypeId == redeemTypeId,
                includeProperties: $"{nameof(Redeem.RedeemType)}"
            )
            .ConfigureAwait(false);

        if (redeems is null)
            return null;

        return redeems
            .Select(redeem => new RedeemDto
            {
                Id = redeem.Id,
                RedeemNo = redeem.RedeemNo,
                RedeemTypeId = redeem.RedeemType.Id,
                RedeemType = new RedeemTypeDto
                {
                    Name = redeem.RedeemType?.Name,
                    Amount = redeem.RedeemType.Amount
                },
                CreatedBy = redeem.CreatedBy?.FullName,
                UpdatedBy = redeem.UpdatedBy?.FullName,
                CreatedDateTime = redeem.CreatedDateTime,
                UpdatedDateTime = redeem.UpdatedDateTime
            })
            .ToList();
    }

    public async Task<List<RedeemDto>> GetRedeemsByStatusAsync(bool isUsed)
    {
        var redeems = await unitOfWork
            .Repository<Redeem>()
            .GetAllAsync(
                redeem => redeem.IsUsed == isUsed,
                includeProperties: $"{nameof(Redeem.RedeemType)}"
            )
            .ConfigureAwait(false);

        if (redeems == null)
            return null;

        return redeems
            .Select(redeem => new RedeemDto
            {
                Id = redeem.Id,
                RedeemNo = redeem.RedeemNo,
                RedeemTypeId = redeem.RedeemTypeId,
                RedeemType = new RedeemTypeDto
                {
                    Name = redeem.RedeemType?.Name,
                    Amount = redeem.RedeemType.Amount
                },
                CreatedBy = redeem.CreatedBy?.FullName,
                UpdatedBy = redeem.UpdatedBy?.FullName,
                CreatedDateTime = redeem.CreatedDateTime,
                UpdatedDateTime = redeem.UpdatedDateTime
            })
            .ToList();
    }

    public async Task<ResponseDto> SubmitRedeemAsync(RedeemDto redeem)
    {
        var generatedRedeems = new List<Redeem>();

        for (int i = 0; i < redeem.Count; i++)
        {
            Redeem newRedeem;
            do
            {
                var randomNo = StringHelper.RandomNumberGenerator(10);
                var checkDuplicateRedeem = await unitOfWork
                    .Repository<Redeem>()
                    .GetAsync(x => x.RedeemNo == randomNo)
                    .ConfigureAwait(false);

                if (checkDuplicateRedeem != null)
                    continue;

                newRedeem = new Redeem
                {
                    RedeemNo = randomNo,
                    RedeemTypeId = redeem.RedeemTypeId,
                    CreatedById = authService.GetCurrentUserId(),
                    UpdatedById = authService.GetCurrentUserId(),
                    CreatedDateTime = DateTime.UtcNow,
                    UpdatedDateTime = DateTime.UtcNow
                };
                break;
            } while (true);
            generatedRedeems.Add(newRedeem);
        }

        await unitOfWork.Repository<Redeem>().AddRangeAsync(generatedRedeems).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return new ResponseDto { Message = "Redeem created successfully", IsSuccess = true };
    }
}
