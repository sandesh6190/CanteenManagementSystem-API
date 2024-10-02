using Canteen.Data.Entities;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.RedeemType;
using Canteen.Shared.Dtos.Response;
using Lib.Authorization.Interfaces;

namespace Canteen.Service;

public class RedeemTypeService(IUnitOfWork unitOfWork, IAuthService authService) : IRedeemTypeService
{  
    public async Task<List<RedeemTypeDto>> GetRedeemTypesAsync()
    {
        var redeemTypes = await unitOfWork
            .Repository<RedeemType>()
            .GetAllAsync(includeProperties:$"{nameof(RedeemType.CreatedBy)},{nameof(RedeemType.UpdatedBy)}")
            .ConfigureAwait(false);

        return redeemTypes.Select(redeemType => new RedeemTypeDto
        {
            Id = redeemType.Id,
            Name = redeemType.Name,
            Amount = redeemType.Amount,
            Description = redeemType.Description,
            CreatedBy = redeemType.CreatedBy?.FullName,
            UpdatedBy = redeemType.UpdatedBy?.FullName,
            CreatedDateTime = redeemType.CreatedDateTime,
            UpdatedDateTime = redeemType.UpdatedDateTime
        }).ToList();
    }

    public async Task<RedeemTypeDto> GetRedeemTypeAsync(int id)
    {
        var redeemType = await unitOfWork
            .Repository<RedeemType>()
            .GetByIdAsync(id)
            .ConfigureAwait(false);

        if (redeemType == null) return null;

        return new RedeemTypeDto
        {
            Id = redeemType.Id,
            Name = redeemType.Name,
            Amount = redeemType.Amount,
            Description = redeemType.Description,
            CreatedBy = redeemType.CreatedById,
            UpdatedBy = redeemType.UpdatedById,
            CreatedDateTime = redeemType.CreatedDateTime,
            UpdatedDateTime = redeemType.UpdatedDateTime
        };
    }

    public async Task<ResponseDto> SubmitRedeemTypeAsync(RedeemTypeDto redeemTypeDto)
    {
        var checkDuplicateName = await unitOfWork
            .Repository<RedeemType>()
            .GetAsync(x=>x.Name.ToUpper().Trim() == redeemTypeDto.Name.ToUpper().Trim())
            .ConfigureAwait(false);

        if(checkDuplicateName != null && redeemTypeDto.Id != checkDuplicateName.Id)        
            return new ResponseDto
            {
                Message = "RedeemType name already exists",
                IsSuccess = false
            };
        
        if(redeemTypeDto.Id > 0)
        {
            var redeemType = await unitOfWork
                .Repository<RedeemType>()
                .GetByIdAsync(redeemTypeDto.Id)
                .ConfigureAwait(false);

            if(redeemType == null) return new ResponseDto { Message ="RedeemType not found", IsSuccess = false };

            redeemType.Name = redeemTypeDto.Name;
            redeemType.Amount = redeemTypeDto.Amount;
            redeemType.Description = redeemTypeDto.Description;
            redeemType.UpdatedById = authService.GetCurrentUserId();
            redeemType.UpdatedDateTime = DateTime.UtcNow;

            await unitOfWork
                .Repository <RedeemType>()
                .UpdateAsync(redeemType)
                .ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync()
                .ConfigureAwait(false);

            return new ResponseDto
            {
                Message = "RedeemType updated successfully",
                IsSuccess = true
            };
        }
        else
        {
            var redeemType = new RedeemType
            {
                Name = redeemTypeDto.Name,
                Amount = redeemTypeDto.Amount,
                Description = redeemTypeDto.Description,
                CreatedById = authService.GetCurrentUserId(),
                UpdatedById = authService.GetCurrentUserId(),
                CreatedDateTime = DateTime.UtcNow,
                UpdatedDateTime = DateTime.UtcNow
            };

            await unitOfWork
                .Repository<RedeemType>()
                .AddAsync(redeemType)
                .ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync() 
                .ConfigureAwait(false);

            return new ResponseDto
            {
                Message = "RedeemType created successfully",
                IsSuccess = true
            };
        }
    }

    public async Task<ResponseDto> DeleteRedeemTypeAsync(int id)
    {
        var redeemType = await unitOfWork
            .Repository<RedeemType>()
            .GetByIdAsync(id)
            .ConfigureAwait(false);

        if (redeemType == null)
            return new ResponseDto
            {
                Message = "RedeemType is not found",
                IsSuccess = false
            };

        await unitOfWork 
            .Repository<RedeemType>()
            .DeleteAsync(redeemType)
            .ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync()
            .ConfigureAwait(false);

        return new ResponseDto
        {
            Message = "RedeemType deleted successfully",
            IsSuccess = true
        };       
    }
}