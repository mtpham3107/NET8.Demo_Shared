using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
using NET8.Demo.GlobalAdmin.Application.Contracts.Requests;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

namespace NET8.Demo.GlobalAdmin.Controllers.Api;

[Route("api/users")]
public class UserController : ApiControllerBase
{
    private readonly IStringLocalizer<UserController> _localizer;
    private readonly IUserService _userService;

    public UserController(IStringLocalizer<UserController> localizer, IUserService userService)
    {
        _localizer = localizer;
        _userService = userService;
    }

    [HttpGet("{userId}")]
    public async ValueTask<ActionResult<UserResponse>> GetById(Guid userId) => Ok(await _userService.GetByIdAsync(userId));

    [HttpGet()]
    public async ValueTask<ActionResult<IEnumerable<UserResponse>>> GetAll() => Ok(await _userService.GetAllAsync());

    [HttpPut]
    public async ValueTask<ActionResult<UserResponse>> Insert([FromBody] UserInsertRequest model) => Ok(await _userService.InsertAsync(model));

    [HttpPost]
    public async ValueTask<ActionResult<UserResponse>> Update([FromBody] UserUpdateRequest model) => Ok(await _userService.UpdateAsync(model));

    [HttpDelete]
    public async ValueTask<ActionResult<bool>> Delete(Guid userId) => Ok(await _userService.DeleteAsync(userId));

    [HttpPost("change-password")]
    public async ValueTask<ActionResult<bool>> ChangePassword([FromBody] UserChangePasswordRequest model) => Ok(await _userService.ChangePasswordAsync(model.Id, model.CurrentPassword, model.NewPassword));

    [HttpPost("reset-password")]
    public async ValueTask<ActionResult<string>> ResetPassword([FromBody] string email) => Ok(await _userService.ResetPasswordAsync(email));
}
