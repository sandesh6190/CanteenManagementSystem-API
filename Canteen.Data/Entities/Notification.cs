using Canteen.Shared.Enums.Notification;

namespace Canteen.Data.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public NotificationType Type { get; set; }
    public NotificationAudience Audience { get; set; }
}
