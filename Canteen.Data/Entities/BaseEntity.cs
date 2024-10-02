using Lib.Authorization.Entities;

namespace Canteen.Data.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public string? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public string? UpdatedById { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime UpdatedDateTime { get; set; }
}