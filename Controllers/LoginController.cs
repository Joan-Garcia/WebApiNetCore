using Microsoft.AspNetCore.Mvc;
using WebApi.DataAcesss;
using WebApi.DataTransfer;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase {
    private readonly ILogger<LoginController> _logger;
    private readonly LoginService _loginService;

    public LoginController(ILogger<LoginController> logger, LoginService loginService) {
        _logger = logger;
        _loginService = loginService;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequestDto request) {
        _logger.LogDebug($"Login user with IdPersonal: {request.IdPersonal}");

        string token = await _loginService.Login(request);

        return Ok(token);
    }
}