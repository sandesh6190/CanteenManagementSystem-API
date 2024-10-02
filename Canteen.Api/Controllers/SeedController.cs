using System.Net;
using Asp.Versioning;
using Lib.Authorization.Dtos;
using Lib.Authorization.Enums;
using Lib.Authorization.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Canteen.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class SeedController(IAuthService authService)
    : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> SeedSuperAdmin()
    {
        var roles = await authService.GetRolesAsync()
            .ConfigureAwait(false);

        if (roles?.Any() == true) return BadRequest("Super Admin Already Exists");

        await authService.SubmitRole(UserRoles.Admin.ToString());
        await authService.SubmitRole(UserRoles.Author.ToString());

        var user = new UserDto
        {
            FirstName = "Super",
            LastName = "Admin",
            Email = "admin@gmail.com",
            UserName = "admin",
            Password = "Admin@123",
            ConfirmPassword = "Admin@123",
            Roles = new List<string> { UserRoles.Admin.ToString() }
        };

        var response = await authService.SubmitUserAsync(user)
            .ConfigureAwait(false);

        return response.IsSuccess ? Ok("SuperAdmin Created Successfully") : BadRequest(response.Message);
    }
}