using System.Net;
using Asp.Versioning;
using Canteen.Service.Interface;
using Canteen.Shared.Dtos.Response;
using Canteen.Shared.Dtos.Unit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Canteen.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class UnitController(IUnitService unitService) : Controller
{
    [HttpGet("getunits")]
    [ProducesResponseType(typeof(UnitDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUnitsAsync()
    {
        var response = await unitService.GetUnitsAsync()
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpGet("getunit")]
    [ProducesResponseType(typeof(UnitDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUnitAsync(int id)
    {
        var response = await unitService.GetUnitAsync(id)
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpPost("submitunit")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SubmitUnitAsync([FromBody] UnitDto unitDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid model state",
                StatusCode = (int)HttpStatusCode.BadRequest
            });

        var response = await unitService.SubmitUnitAsync(unitDto)
            .ConfigureAwait(false);
        return Ok(response);
    }

    [HttpDelete("deleteunit")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteUnitAsync(int id)
    {
        var response = await unitService.DeleteUnitAsync(id)
            .ConfigureAwait(false);
        return Ok(response);
    }
}