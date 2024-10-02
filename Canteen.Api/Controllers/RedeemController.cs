using Asp.Versioning;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Redeem;
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
public class RedeemController(IRedeemService redeemService) : Controller
{
    [HttpGet("getredeems")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetRedeemsAsync()
    {
        var response = await redeemService.GetRedeemsAsync()
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getredeemsbyredeemtype")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetRedeemsByRedeemTypeAsync(int redeemTypeId)
    {
        if (redeemTypeId <= 0)
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid redeemType id",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        var response = await redeemService.GetRedeemsByRedeemTypeAsync(redeemTypeId)
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getredeemsbystatus")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetRedeemsByStatusAsync(bool isUsed)
    {
        var response = await redeemService.GetRedeemsByStatusAsync(isUsed)
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpPost("submitredeem")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SubmitRedeemAsync(RedeemDto redeem)
    {
        if (redeem.RedeemType == null || redeem.Count <= 0 )
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid data",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        var response = await redeemService.SubmitRedeemAsync(redeem)
            .ConfigureAwait(false);
        return Ok(response);
    }
}