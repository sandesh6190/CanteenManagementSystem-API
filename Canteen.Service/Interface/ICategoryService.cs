using Canteen.Shared.Dtos.Category;
using Canteen.Shared.Dtos.Response;

namespace Canteen.Service.Interface;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetCategoriesAsync();
    Task<CategoryDto> GetCategoryAsync(int id);
    Task<ResponseDto> SubmitCategoryAsync(CategoryDto categoryDto);
    Task<ResponseDto> DeleteCategoryAsync(int id);
}