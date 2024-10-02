using System;
using Canteen.Shared.Dtos.Notification;
using Canteen.Shared.Dtos.Notification.NotificationStatus;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Enums.Notification;

namespace Canteen.Service.Interface;

public interface INotificationService
{
    Task<List<NotificationDto>> GetNotificationsAsync();
    Task<List<NotificationDto>> GetNotificationsAsync(NotificationType notificationType);
    Task<List<NotificationDto>> GetNotificationsAsync(NotificationAudience notificationAudience);
    Task<List<NotificationDto>> GetNotificationsByUserAsync();
    Task<ResponseDto> SubmitNotificationAsync(NotificationDto notification);
    Task<ResponseDto> SubmitNotificationStatusAsync(int notificationId);
}
