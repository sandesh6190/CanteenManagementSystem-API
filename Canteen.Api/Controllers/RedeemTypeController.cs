using Asp.Versioning;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.RedeemType;
using Canteen.Shared.Dtos.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Canteen.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class RedeemTypeController(IRedeemTypeService redeemTypeService) : Controller
{
    [HttpGet("getredeemtypes")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetRedeemTypesAsync()
    {
        var response = await redeemTypeService.GetRedeemTypesAsync()
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getredeemtype")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetRedeemTypeAsync(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Username Required",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }

        var response = await redeemTypeService.GetRedeemTypeAsync(id)
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpPost("submitredeemtype")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SubmitRedeemType([FromBody] RedeemTypeDto redeemType)
    {
        if (string.IsNullOrWhiteSpace(redeemType.Name) || redeemType.Amount <= 0)
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid data",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }

        var response = await redeemTypeService.SubmitRedeemTypeAsync(redeemType)
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpDelete("deleteredeemtype")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteRedeemTypeAsync(int id)
    {
        var response = await redeemTypeService.DeleteRedeemTypeAsync(id)
            .ConfigureAwait(false);
        return Ok(response);
    }
}