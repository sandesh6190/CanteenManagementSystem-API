using Lib.Authorization.Entities;

namespace Canteen.Data.Entities;

public class UserBalance
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; } 
    public decimal Balance { get; set; }  
}