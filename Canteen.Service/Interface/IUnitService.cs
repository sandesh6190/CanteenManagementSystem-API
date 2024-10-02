using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Dtos.Unit;

namespace Canteen.Service.Interface;

public interface IUnitService
{
    Task<List<UnitDto>> GetUnitsAsync();
    Task<UnitDto> GetUnitAsync(int id);
    Task<ResponseDto> SubmitUnitAsync(UnitDto unitDto);
    Task<ResponseDto> DeleteUnitAsync(int id);
}