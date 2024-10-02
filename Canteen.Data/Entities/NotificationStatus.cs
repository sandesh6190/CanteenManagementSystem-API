using System;
using Lib.Authorization.Entities;
using Microsoft.Identity.Client;

namespace Canteen.Data.Entities;

public class NotificationStatus
{
    public int Id { get; set; }
    public int NotificationId { get; set; }
    public Notification? Notification { get; set; }
    public string UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
