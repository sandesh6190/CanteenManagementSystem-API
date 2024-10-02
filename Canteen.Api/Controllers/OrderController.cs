using System.Net;
using Asp.Versioning;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Notification;
using Canteen.Shared.Dtos.Order;
using Canteen.Shared.Dtos.OrderDetails;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Canteen.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class OrderController(IOrderService orderService) : Controller
{
    [HttpPost("submitorder")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)] //ya pani response chai return garnu parne haina?
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SubmitOrderAsync([FromBody] OrderDto orderDto)
    {
        foreach (var order in orderDto.OrderDetails)
        {
            if (order.ItemId < 1)
                return BadRequest("Invalid item id");
        }
        var response = await orderService.SubmitOrderAsync(orderDto).ConfigureAwait(false);
        return response.IsSuccess? Ok(response): BadRequest((response));
    }

    [HttpGet("getorderbyuserid")]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetOrderByUserIdAsync(string userId)
    {
        var response = await orderService.GetOrderByUserIdAsync(userId).ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getorderbyid")]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetOrderByIdAsync(int id)
    {
        if (id < 1)
            return BadRequest(new ResponseDto { Message = "Invalid id", IsSuccess = false });
        var response = await orderService.GetOrderByIdAsync(id).ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getorderbystatus")]
    [ProducesResponseType(typeof(OrderDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetOrderByStatusAsync(OrderStatus orderStatus)
    {
        var response = await orderService.GetOrderByStatusAsync(orderStatus).ConfigureAwait(false);
        return Ok(response);
    }

    [HttpPut("changeorderstatus")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)] //ya pani responseDto return hunu parne haina?
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ChangeOrderStatusAsync(int orderId, OrderStatus orderStatus)
    {
        var response = await orderService
            .ChangeOrderStatusAsync(orderId, orderStatus)
            .ConfigureAwait(false);
        
        return response.IsSuccess? Ok(response): BadRequest(response);
    }
    [HttpGet("getorderdetailsbyorderid")]
    [ProducesResponseType(typeof(OrderDetailsDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetOrderDetailsByOrderIdAsync(int orderId)
    {
        if (orderId <= 0)
        {
            return BadRequest(new ResponseDto { Message = "Invalid id", IsSuccess = false });
        }
        var response = await orderService.GetOrderDetailsByOrderIdAsync(orderId).ConfigureAwait(false);
        return Ok(response);
    }
}
