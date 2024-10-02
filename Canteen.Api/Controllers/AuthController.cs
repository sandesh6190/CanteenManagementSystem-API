using System.Net;
using Asp.Versioning;
using Lib.Authorization.Dtos;
using Lib.Authorization.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Canteen.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : Controller
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto login)
    {
        var response = await authService.LoginAsync(login)
            .ConfigureAwait(false);
        return response.IsAuthSuccessful ? Ok(response) : BadRequest(response);
    }

    [AllowAnonymous]
    [HttpPost("refreshtoken")]
    [ProducesResponseType(typeof(AuthResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshToken)
    {
        var response = await authService.RefreshToken(refreshToken)
            .ConfigureAwait(false);
        return response.IsAuthSuccessful ? Ok(response) : BadRequest(response);
    }

    [HttpGet("getroles")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetRolesAsync()
    {
        var roles = await authService.GetRolesAsync()
            .ConfigureAwait(false);
        return Ok(roles);
    }

    [HttpGet("getrolesbyuser")]
    [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetRolesByUserAsync(string userId)
    {
        var roles = await authService.GetRolesAsync(userId)
            .ConfigureAwait(false);
        return Ok(roles);
    }

    [HttpGet("getuser")]
    [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUserAsync(string id)
    {
        var response = await authService.GetUserAsync(id)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [HttpGet("getusers")]
    [ProducesResponseType(typeof(List<UserDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetUsersAsync()
    {
        var response = await authService.GetUsersAsync()
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [HttpPost("submituser")]
    [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserDto user)
    {
        if (string.IsNullOrWhiteSpace(user.Id) && string.IsNullOrWhiteSpace(user.Password))
            return BadRequest(new UserResponseDto
            {
                IsSuccess = false,
                Message = "Password is required"
            });

        var response = await authService.SubmitUserAsync(user)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [HttpDelete("deleteuser")]
    [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteUserAsync(string id)
    {
        var response = await authService.DeleteUserAsync(id)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [HttpPost("addroletoUser")]
    [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> AddRoleToUserAsync(RoleUserDto roleUser)
    {
        var response = await authService.AddRoleToUserAsync(roleUser)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("forgotpassword")]
    [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgetPasswordDto forgotPassword)
    {
        var response = await authService.ForgotPasswordAsync(forgotPassword)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [HttpPost("changepassword")]
    [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePassword)
    {
        var response = await authService.ChangePasswordAsync(changePassword)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [AllowAnonymous]
    [HttpPost("resetpassword")]
    [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto resetPassword)
    {
        var response = await authService.ResetPasswordAsync(resetPassword)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }

    [HttpPost("resetpasswordbyadmin")]
    [ProducesResponseType(typeof(UserResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> ResetPasswordByAdminAsync([FromBody] ResetPasswordByAdminDto resetPassword)
    {
        var response = await authService.ResetPasswordByAdminAsync(resetPassword)
            .ConfigureAwait(false);
        return response != null ? Ok(response) : BadRequest();
    }
}