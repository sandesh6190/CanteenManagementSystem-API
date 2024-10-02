using Canteen.Shared.Dtos.Category;

namespace Canteen.Shared.Dtos.Item;

public class ItemDto : BaseDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Decimal Price { get; set; }
    public int CategoryId { get; set; }
    public CategoryDto Category { get; set; }
}
