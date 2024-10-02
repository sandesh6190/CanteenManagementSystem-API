using Canteen.Data.Entities;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Dtos.Unit;
using Lib.Authorization.Interfaces;

namespace Canteen.Service;

public class UnitService(IUnitOfWork unitOfWork, IAuthService authService) : IUnitService
{
    public async Task<List<UnitDto>> GetUnitsAsync()
    {
        var units = await unitOfWork
            .Repository<Unit>()
            .GetAllAsync(includeProperties: $"{nameof(Unit.CreatedBy)}, {nameof(Unit.UpdatedBy)}")
            .ConfigureAwait(false);

        return units.Select(unit => new UnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Description = unit.Description,
            CreatedBy = unit.CreatedBy?.FullName,
            UpdatedBy = unit.UpdatedBy?.FullName,
            CreatedDateTime = unit.CreatedDateTime,
            UpdatedDateTime = unit.UpdatedDateTime
        }).ToList();
    }

    public async Task<UnitDto> GetUnitAsync(int id)
    {
        var unit = await unitOfWork
            .Repository<Unit>()
            .GetByIdAsync(id)
            .ConfigureAwait(false);

        if (unit == null) return null;

        return new UnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Description = unit.Description,
            CreatedBy = unit.CreatedBy?.FullName,
            UpdatedBy = unit.UpdatedBy?.FullName,
            CreatedDateTime = unit.CreatedDateTime,
            UpdatedDateTime = unit.UpdatedDateTime
        };
    }

    public async Task<ResponseDto> SubmitUnitAsync(UnitDto unitDto)
    {
        var checkDuplicateName = await unitOfWork
            .Repository<Unit>()
            .GetAsync(x => x.Name.ToUpper().Trim() == unitDto.Name.ToUpper().Trim())
            .ConfigureAwait(false);

        if (checkDuplicateName is not null && unitDto.Id != checkDuplicateName.Id)
            return new ResponseDto
            {
                Message = "Unit name already exists",
                IsSuccess = false
            };

        if (unitDto.Id > 0)
        {
            var unit = await unitOfWork
                .Repository<Unit>()
                .GetByIdAsync(unitDto.Id)
                .ConfigureAwait(false);

            if (unit == null) return new ResponseDto { Message = "Unit not found", IsSuccess = false };

            unit.Name = unitDto.Name;
            unit.Description = unitDto.Description;
            unit.UpdatedById = authService.GetCurrentUserId();
            unit.UpdatedDateTime = DateTime.UtcNow;

            await unitOfWork
                .Repository<Unit>()
                .UpdateAsync(unit)
                .ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync()
                .ConfigureAwait(false);

            return new ResponseDto
            {
                Message = "Unit updated successfully",
                IsSuccess = true
            };
        }
        else
        {
            var unit = new Unit
            {
                Name = unitDto.Name,
                Description = unitDto.Description,
                CreatedById = authService.GetCurrentUserId(),
                UpdatedById = authService.GetCurrentUserId(),
                CreatedDateTime = DateTime.UtcNow,
                UpdatedDateTime = DateTime.UtcNow
            };

            await unitOfWork
                .Repository<Unit>()
                .AddAsync(unit)
                .ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync()
                .ConfigureAwait(false);

            return new ResponseDto
            {
                Message = "Unit created successfully",
                IsSuccess = true
            };
        }
    }

    public async Task<ResponseDto> DeleteUnitAsync(int id)
    {
        var unit = await unitOfWork
            .Repository<Unit>()
            .GetByIdAsync(id)
            .ConfigureAwait(false);

        if (unit == null)
            return new ResponseDto
            {
                Message = "Unit not found",
                IsSuccess = false
            };

        await unitOfWork
            .Repository<Unit>()
            .DeleteAsync(unit)
            .ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync()
            .ConfigureAwait(false);

        return new ResponseDto
        {
            Message = "Unit deleted successfully",
            IsSuccess = true
        };
    }
}