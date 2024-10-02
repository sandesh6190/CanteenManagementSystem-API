using Canteen.Data.Entities;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Category;
using Canteen.Shared.Dtos.Response;
using Lib.Authorization.Interfaces;

namespace Canteen.Service;

public class CategoryService(IUnitOfWork unitOfWork, IAuthService authService) : ICategoryService
{
    public async Task<List<CategoryDto>> GetCategoriesAsync()
    {
        var categories = await unitOfWork
            .Repository<Category>()
            .GetAllAsync(includeProperties: $"{nameof(Category.CreatedBy)},{nameof(Category.UpdatedBy)}")
            .ConfigureAwait(false);

        return categories.Select(category => new CategoryDto
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedBy = category.CreatedBy?.FullName,
            UpdatedBy = category.UpdatedBy?.FullName,
            CreatedDateTime = category.CreatedDateTime,
            UpdatedDateTime = category.UpdatedDateTime
        }).ToList();
    }

    public async Task<CategoryDto> GetCategoryAsync(int id)
    {
        var category =await unitOfWork
            .Repository<Category>()
            .GetByIdAsync(id)
            .ConfigureAwait(false);

        if (category == null) return null;

        return new CategoryDto
        {
            Id = category.Id,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedBy = category.CreatedBy?.FullName,
            UpdatedBy = category.UpdatedBy?.FullName,
            CreatedDateTime = category.CreatedDateTime,
            UpdatedDateTime = category.UpdatedDateTime
        };
    }

    public async Task<ResponseDto> SubmitCategoryAsync(CategoryDto categoryDto)
    {
        var checkDuplicateName = await unitOfWork
            .Repository<Category>()
            .GetAsync(x=>x.CategoryName.ToUpper().Trim()==categoryDto.CategoryName.ToUpper().Trim())
            .ConfigureAwait(false);

        if(checkDuplicateName != null && categoryDto.Id != checkDuplicateName.Id) 
            return new ResponseDto
            {
                Message = "Category Name already exists.",
                IsSuccess = false
            };

        if(categoryDto.Id > 0) 
        {
            var category = await unitOfWork
            .Repository<Category>()
            .GetByIdAsync(categoryDto.Id)
            .ConfigureAwait(false);

            if (category == null) return new ResponseDto { Message = "Category not Found", IsSuccess = false };

            category.CategoryName = categoryDto.CategoryName;
            category.Description = categoryDto.Description;
            category.UpdatedById = authService.GetCurrentUserId();
            category.UpdatedDateTime = DateTime.UtcNow;

            await unitOfWork
                .Repository<Category>()
                .UpdateAsync(category)
                .ConfigureAwait(false);

            await unitOfWork
                .SaveChangesAsync()
                .ConfigureAwait(false);

            return new ResponseDto
            {
                Message = "Category Updated Successfully",
                IsSuccess = true
            };
        }
        else
        {
            var category = new Category
            {
                CategoryName = categoryDto.CategoryName,
                Description = categoryDto.Description,
                CreatedById = authService.GetCurrentUserId(),
                UpdatedById = authService.GetCurrentUserId(),
                CreatedDateTime = DateTime.UtcNow,
                UpdatedDateTime = DateTime.UtcNow
            };

            await unitOfWork
                .Repository<Category>()
                .AddAsync(category)
                .ConfigureAwait(false);

            await unitOfWork
                .SaveChangesAsync() 
                .ConfigureAwait(false);

            return new ResponseDto
            {
                Message = "Category Created Successfully",
                IsSuccess = true
            };
        }
    }

    public async Task<ResponseDto> DeleteCategoryAsync(int id)
    {
        var category = await unitOfWork
            .Repository<Category>()
            .GetByIdAsync(id)
            .ConfigureAwait(false);

        if (category == null)
            return new ResponseDto
            {
                Message = "Category not Found",
                IsSuccess = false
            };
            
        await unitOfWork
            .Repository<Category>()
            .DeleteAsync(id)
            .ConfigureAwait(false);

        await unitOfWork
            .SaveChangesAsync() 
            .ConfigureAwait(false);

        return new ResponseDto
        {
            Message = "Category Deleted Successfully",
            IsSuccess = true
        };
    }
}