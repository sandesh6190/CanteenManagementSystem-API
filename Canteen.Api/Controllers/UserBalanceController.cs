using Asp.Versioning;
using Canteen.Service.Interface;
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
public class UserBalanceController(IUserBalanceService userBalanceService) : Controller
{
    [HttpGet("getbalances")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetBalancesAsync()
    {
        var response = await userBalanceService.GetBalancesAsync()
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getbalance")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetBalanceAsync(string userId)
    {
        if(string.IsNullOrWhiteSpace(userId))
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid userId",
                StatusCode = (int)HttpStatusCode.BadRequest
            });   

        var response = await userBalanceService.GetBalanceAsync(userId)
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpPost("submitbalance")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SubmitBalanceAsync(string redeemNumber)
    {
        if (string.IsNullOrWhiteSpace(redeemNumber))
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid data",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }

        var response = await userBalanceService.SubmitBalanceAsync(redeemNumber)
            .ConfigureAwait(false);
        return Ok(response);
    }
}