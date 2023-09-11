using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DataAcesss;
using WebApi.DataTransfer;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
    private readonly ILogger<UserController> _logger;
    private readonly UserDao _userDao;
    private readonly UserService _userService;

    public UserController(ILogger<UserController> logger, UserDao userDao, UserService userService) {
        _logger = logger;
        _userDao = userDao;
        _userService = userService;
    }

    [Authorize]
    [HttpGet("AllUsers")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUser() {
        _logger.LogDebug("Getting all users");

        // throw new HttpException(HttpStatusCode.BadRequest, "Error test");
        
        return Ok(await _userDao.GetUsers());
    }

    [HttpGet("{IdPersonal}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUser(int IdPersonal) {
        _logger.LogDebug($"Getting user with IdPersonal: {IdPersonal}");

        var user = await _userDao.GetUser(IdPersonal);
        
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
    public async Task<ActionResult<int>> CreateUser(UserDto user) {
        _logger.LogDebug($"Creating user with IdPersonal: {user.IdPersonal}");

        int idUser = await _userDao.CreateUser(user);

        return CreatedAtAction(nameof(CreateUser), new { IdPersonal = idUser }, user);
    }

    [HttpPut]
    public async Task<ActionResult<int>> UpdateUser(UserDto user) {
        _logger.LogDebug($"Updating user with IdPersonal: {user.IdPersonal}");
        return Ok(await _userDao.UpdateUser(user));
    }

    [HttpDelete("{IdPersonal}")]
    public async Task<ActionResult<int>> DeleteUser(int IdPersonal) {
        _logger.LogDebug($"Deleting user with IdPersonal: {IdPersonal}");
        return Ok(await _userDao.DeleteUser(IdPersonal));
    }

    [HttpGet("TestTransaction")]
    public async Task<ActionResult<string>> TestTransaction() {
        _logger.LogDebug("Testing transaction");

        await _userService.TestTransaction();

        return Ok("Ok");
    }

    [HttpGet("TestRedis")]
    public async Task<ActionResult<string>> TestRedis() {
        _logger.LogDebug("Testing Redis");

        var x = await _userService.TestRedis();

        return Ok(x);
    }
}