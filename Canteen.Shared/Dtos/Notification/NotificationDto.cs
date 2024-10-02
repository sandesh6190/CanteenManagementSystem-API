using System;
using Canteen.Shared.Enums.Notification;

namespace Canteen.Shared.Dtos.Notification;

public class NotificationDto : BaseDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public NotificationType Type { get; set; }
    public NotificationAudience Audience { get; set; }
    public bool IsViewed { get; set; }
}
