using System;
using Canteen.Data.Entities;
using Canteen.Data.UnitOfWork.Interfaces;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Notification;
using Canteen.Shared.Dtos.Notification.NotificationStatus;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Enums.Notification;
using Lib.Authorization.Enums;
using Lib.Authorization.Interfaces;

namespace Canteen.Service;

public class NotificationService(IUnitOfWork unitOfWork, IAuthService authService) : INotificationService
{
    public async Task<List<NotificationDto>> GetNotificationsAsync()
    {
        var notifications = await unitOfWork.Repository<Notification>().GetAllAsync(includeProperties: $"{nameof(Notification.CreatedBy)},{nameof(Notification.UpdatedBy)}").ConfigureAwait(false);

        return notifications.Select(notification => new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Description = notification.Description,
            Type = notification.Type,
            Audience = notification.Audience,
            CreatedBy = notification.CreatedBy?.FullName,
            UpdatedBy = notification.UpdatedBy?.FullName,
            CreatedDateTime = notification.CreatedDateTime,
            UpdatedDateTime = notification.UpdatedDateTime
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(NotificationType notificationType)
    {
        var notifications = await unitOfWork.Repository<Notification>().GetAllAsync(x => x.Type == notificationType).ConfigureAwait(false);

        return notifications.Select(notification => new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Description = notification.Description,
            Type = notification.Type,
            Audience = notification.Audience,
            CreatedBy = notification.CreatedBy?.FullName,
            UpdatedBy = notification.UpdatedBy?.FullName,
            CreatedDateTime = notification.CreatedDateTime,
            UpdatedDateTime = notification.UpdatedDateTime
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(NotificationAudience notificationAudience)
    {
        var notifications = await unitOfWork.Repository<Notification>().GetAllAsync(x => x.Audience == notificationAudience).ConfigureAwait(false);

        return notifications.Select(notification => new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Description = notification.Description,
            Type = notification.Type,
            Audience = notification.Audience,
            CreatedBy = notification.CreatedBy?.FullName,
            UpdatedBy = notification.UpdatedBy?.FullName,
            CreatedDateTime = notification.CreatedDateTime,
            UpdatedDateTime = notification.UpdatedDateTime
        }).ToList();
    }

    public async Task<List<NotificationDto>> GetNotificationsByUserAsync()
    {
        var userId = authService.GetCurrentUserId();
        var userRoles = await authService.GetRolesAsync(userId);
        var notifications = new List<NotificationDto>();
        
        if (userRoles.Contains("Author"))
        {
            notifications = await GetNotificationsAsync(NotificationAudience.All);
        }
        else if (userRoles.Contains("Accountant"))
            notifications = await GetNotificationsAsync(NotificationAudience.Accountant);
        else if (userRoles.Contains("Customer"))
            notifications = await GetNotificationsAsync(NotificationAudience.Customer);

        notifications.ForEach(x =>
        {
            var notificationStatus = GetNotificationStatusByUserIdAndNotifyIdAsync(userId, x.Id).GetAwaiter().GetResult();
            if (notificationStatus != null)
            {
                x.IsViewed = true;
            }
        });
        return notifications;
    }
    
    public async Task<ResponseDto> SubmitNotificationAsync(NotificationDto notification)
    {
        var checkDuplicateNotification = await unitOfWork.Repository<Notification>().GetAsync(x => x.Title.ToUpper().Trim() == notification.Title.ToUpper().Trim()).ConfigureAwait(false);

        if (checkDuplicateNotification is not null && checkDuplicateNotification.Id != notification.Id)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Notification Title already exist."
            };
        }

        if (notification.Id > 0)
        {
            var notify = await unitOfWork.Repository<Notification>().GetByIdAsync(checkDuplicateNotification.Id).ConfigureAwait(false);

            if (notify is null) return new ResponseDto
            {
                IsSuccess = false,
                Message = "No Title Found."
            };

            notify.Title = notification.Title;
            notify.Description = notification.Description;
            notify.Type = notification.Type;
            notify.Audience = notification.Audience;
            notify.UpdatedById = authService.GetCurrentUserId();
            notify.UpdatedDateTime = DateTime.UtcNow;

            await unitOfWork.Repository<Notification>().UpdateAsync(notify).ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Notification Updated."
            };
        }

        else
        {
            var notify = new Notification
            {
                Title = notification.Title,
                Description = notification.Description,
                Type = notification.Type,
                Audience = notification.Audience,
                CreatedById = authService.GetCurrentUserId(),
                UpdatedById = authService.GetCurrentUserId(),
                CreatedDateTime = DateTime.UtcNow,
                UpdatedDateTime = DateTime.UtcNow
            };

            await unitOfWork.Repository<Notification>().AddAsync(notify).ConfigureAwait(false);

            await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = true,
                Message = "Notification Added."
            };
        }
    }
    private async Task<List<NotificationStatusDto>> GetNotificationStatusByUserIdAsync(string userId)
    {
        var notificationStatuses = await unitOfWork.Repository<NotificationStatus>().GetAllAsync(x => x.UserId == userId).ConfigureAwait(false);

        return notificationStatuses.Select(notificationStatus => new NotificationStatusDto
        {
            NotificationId = notificationStatus.NotificationId,
            UserId = notificationStatus.UserId,
        }).ToList();
    }

    private async Task<NotificationStatusDto> GetNotificationStatusByUserIdAndNotifyIdAsync(string userId, int notifyId)
    {
        var notificationStatus = await unitOfWork.Repository<NotificationStatus>().GetAsync(x => x.UserId == userId && x.NotificationId == notifyId).ConfigureAwait(false);

        if (notificationStatus == null) return null;

        return new NotificationStatusDto
        {
            NotificationId = notificationStatus.NotificationId,
            UserId = notificationStatus.UserId,
        };
    }

    public async Task<ResponseDto> SubmitNotificationStatusAsync(int notificationId)
    {
        var userId = authService.GetCurrentUserId();

        var notificationStatus = await unitOfWork.Repository<NotificationStatus>().GetAsync(x => x.NotificationId == notificationId && x.UserId == userId).ConfigureAwait(false);

        if (notificationStatus != null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Notification already read."
            };
        }
        var notifyStatus = new NotificationStatus
        {
            NotificationId = notificationId,
            UserId = userId
        };
        await unitOfWork.Repository<NotificationStatus>().AddAsync(notifyStatus).ConfigureAwait(false);


        await unitOfWork.SaveChangesAsync().ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Notification status is read."
        };
    }
}
