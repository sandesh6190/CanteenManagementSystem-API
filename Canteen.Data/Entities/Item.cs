using Microsoft.EntityFrameworkCore;

namespace Canteen.Data.Entities;

public class Item : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Decimal Price { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}
