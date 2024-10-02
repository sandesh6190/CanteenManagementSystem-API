using Canteen.Shared.Dtos.Item;
using Canteen.Shared.Dtos.Response;

namespace Canteen.Service.Interface;

public interface IItemService
{
    Task<ItemDto> GetItemAsync(int id);
    Task<List<ItemDto>> GetItemsAsync();
    Task<ResponseDto> SubmitItemAsync(ItemDto itemDto);
    Task<ResponseDto> DeleteItemAsync(int id);
    Task<List<ItemDto>> GetItemsByCategoryIdAsync(int categoryId);
}
