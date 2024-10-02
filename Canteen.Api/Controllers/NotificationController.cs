using System.Net;
using Asp.Versioning;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Notification;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Enums.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Canteen.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class NotificationController(INotificationService notificationService) : ControllerBase
    {
        [HttpGet("getnotifications")]
        [ProducesResponseType(typeof(NotificationDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationsAsync()
        {
            var response = await notificationService.GetNotificationsAsync().ConfigureAwait(false);

            return Ok(response);
        }

        [HttpGet("getnotificationsbytype")]
        [ProducesResponseType(typeof(NotificationDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationsByType(NotificationType notificationType)
        {
            var response = await notificationService.GetNotificationsAsync(notificationType).ConfigureAwait(false);

            return Ok(response);
        }

        [HttpGet("getnotificationsbyaudience")]
        [ProducesResponseType(typeof(NotificationDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificatoinsByAudeince(NotificationAudience notificationAudience)
        {
            var response = await notificationService.GetNotificationsAsync(notificationAudience).ConfigureAwait(false);
            return Ok(response);
        }

        [HttpPost("submitnotification")]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SubmitNotification([FromBody] NotificationDto notificationDto)
        {
            if (string.IsNullOrWhiteSpace(notificationDto.Title))
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Title is empty."
                });
            }

            var response = await notificationService.SubmitNotificationAsync(notificationDto).ConfigureAwait(false);

            return Ok(response);
        }

        [HttpGet("getnotificationsbyuser")]
        [ProducesResponseType(typeof(NotificationDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetNotificationsByUserAsync()
        {
            var response = await notificationService.GetNotificationsByUserAsync().ConfigureAwait(false);

            return Ok(response);
        }

        [HttpPost("readnotification")]
        [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ReadNotificationAsync([FromBody] int notificationId)
        {
            if (notificationId <= 0)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Notification Id is null."
                });
            }

            var response = await notificationService.SubmitNotificationStatusAsync(notificationId).ConfigureAwait(false);

            return Ok(response);
        }
    }
}
