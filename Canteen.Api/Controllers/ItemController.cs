using System.Net;
using Asp.Versioning;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Item;
using Canteen.Shared.Dtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Canteen.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class ItemController(IItemService itemService) : Controller
{
    [HttpGet("getitem")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetItemAsync(int id)
    {
        var response = await itemService.GetItemAsync(id).ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getitems")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetItemsAsync()
    {
        var response = await itemService.GetItemsAsync().ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getitemsbycategoryid")]
    [ProducesResponseType(typeof(ItemDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetItemsByCategoryId(int CategoryId)
    {
        var items = await itemService.GetItemsByCategoryIdAsync(CategoryId).ConfigureAwait(false);

        return Ok(items);
    }

    [HttpPost("submititem")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SubmitItemAsync([FromBody] ItemDto itemDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(
                new ResponseDto
                {
                    Message = "Invalid model state",
                    IsSuccess = false,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                }
            );
        var response = await itemService.SubmitItemAsync(itemDto).ConfigureAwait(false);
        return Ok(response);
    }

    [HttpDelete("deleteitem")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteItemAsync(int id)
    {
        var response = await itemService.DeleteItemAsync(id).ConfigureAwait(false);
        return Ok(response);
    }
}