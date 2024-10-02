using Canteen.Data.Entities;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Category;
using Canteen.Shared.Dtos.Item;
using Canteen.Shared.Dtos.Response;
using Lib.Authorization.Interfaces;

namespace Canteen.Service;

public class ItemService(IUnitOfWork unitOfWork, IAuthService authService) : IItemService
{
    public async Task<ResponseDto> DeleteItemAsync(int id)
    {
        var item = await unitOfWork.Repository<Item>().GetByIdAsync(id).ConfigureAwait(false);

        if (item == null)
            return new ResponseDto { Message = "Item not found.", IsSuccess = false };

        await unitOfWork.Repository<Item>().DeleteAsync(item).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return new ResponseDto
        {
            Message = "Item deleted sucessfully",
            IsSuccess = true
        };
    }

    public async Task<ItemDto> GetItemAsync(int id)
    {
        var item = await unitOfWork
            .Repository<Item>()
            .GetAsync(
                x => x.Id == id,
                includeProperties: $"{nameof(Category)},{nameof(Item.CreatedBy)},{nameof(Item.UpdatedBy)}"
            )
            .ConfigureAwait(false);

        if (item == null) return null;

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            CategoryId = item.CategoryId,
            CreatedBy = item.CreatedBy?.FullName,
            UpdatedBy = item.UpdatedBy?.FullName,
            CreatedDateTime = item.CreatedDateTime,
            UpdatedDateTime = item.UpdatedDateTime,
            Category = new CategoryDto
            {
                CategoryName = item.Category.CategoryName,
                Description = item.Category.Description
            }
        };
    }

    public async Task<List<ItemDto>> GetItemsAsync()
    {
        var items = await unitOfWork
            .Repository<Item>()
            .GetAllAsync(
                includeProperties: $"{nameof(Item.CreatedBy)},{nameof(Item.CreatedBy)},{nameof(Category)}"
            )
            .ConfigureAwait(false);
        return items
            .Select(item => new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                CategoryId = item.CategoryId,
                Price = item.Price,
                CreatedBy = item.CreatedBy?.FullName,
                UpdatedBy = item.UpdatedBy?.FullName,
                CreatedDateTime = item.CreatedDateTime,
                UpdatedDateTime = item.UpdatedDateTime,
                Category = new CategoryDto
                {
                    CategoryName = item.Category.CategoryName,
                    Description = item.Category.Description
                }
            })
            .ToList();
    }

    public async Task<List<ItemDto>> GetItemsByCategoryIdAsync(int categoryId)
    {
        var items = await unitOfWork
            .Repository<Item>()
            .GetAllAsync(
                x => x.CategoryId == categoryId,
                includeProperties: $"{nameof(Item.CreatedBy)},{nameof(Category)}"
            )
            .ConfigureAwait(false);
        return items
            .Select(item => new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                CategoryId = item.CategoryId,
                Price = item.Price,
                CreatedBy = item.CreatedBy?.FullName,
                UpdatedBy = item.UpdatedBy?.FullName,
                CreatedDateTime = item.CreatedDateTime,
                UpdatedDateTime = item.UpdatedDateTime,
                Category = new CategoryDto
                {
                    CategoryName = item.Category.CategoryName,
                    Description = item.Category.Description
                }
            })
            .ToList();
    }

    public async Task<ResponseDto> SubmitItemAsync(ItemDto itemDto)
    {
        var categoryExist = await unitOfWork
            .Repository<Category>()
            .GetByIdAsync(itemDto.CategoryId)
            .ConfigureAwait(false);
        if (categoryExist is null)
            return new ResponseDto { Message = "Category Id is not valid", IsSuccess = false };

        var checkDuplicateName = await unitOfWork
            .Repository<Item>()
            .GetAsync(x => x.Name.ToUpper().Trim() == itemDto.Name.ToUpper().Trim())
            .ConfigureAwait(false);

        if (checkDuplicateName is not null && itemDto.Id != checkDuplicateName.Id)
            return new ResponseDto { Message = "Item name already exist", IsSuccess = false };

        if (itemDto.Id > 0)
        {
            var item = await unitOfWork
                .Repository<Item>()
                .GetByIdAsync(itemDto.Id)
                .ConfigureAwait(false);

            if (item is null)
                return new ResponseDto { Message = "Item not found!", IsSuccess = false };

            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.CategoryId = itemDto.CategoryId;
            item.Price = itemDto.Price;
            item.UpdatedById = authService.GetCurrentUserId();
            item.UpdatedDateTime = DateTime.UtcNow;

            await unitOfWork.Repository<Item>().UpdateAsync(item).ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return new ResponseDto { Message = "Item updated successfully", IsSuccess = true };
        }

        var newItem = new Item
        {
            Name = itemDto.Name,
            Description = itemDto.Description,
            Price = itemDto.Price,
            CategoryId = itemDto.CategoryId,
            CreatedById = authService.GetCurrentUserId(),
            UpdatedById = authService.GetCurrentUserId(),
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow
        };

        await unitOfWork.Repository<Item>().AddAsync(newItem).ConfigureAwait(false);

        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return new ResponseDto { Message = "Item added successfully", IsSuccess = true };
    }
}