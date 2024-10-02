using System;

namespace Canteen.Shared.Dtos.Notification.NotificationStatus;

public class NotificationStatusDto
{
    public int NotificationId { get; set; }
    public string UserId { get; set; }
    public bool IsRead { get; set; }
}
