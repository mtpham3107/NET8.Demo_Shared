using Microsoft.AspNetCore.Mvc;
using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;

namespace NET8.Demo.GlobalAdmin.Controllers.Api;

[Route("api/emails")]
public class EmailController : ApiControllerBase
{
    private readonly IEmailService _service;

    public EmailController(IEmailService service)
    {
        _service = service;
    }

    [HttpGet("send-created-account")]
    public async ValueTask SendAccountCreated(string email, string password) => await _service.SendAccountCreatedEmailAsync(email, password);
}
