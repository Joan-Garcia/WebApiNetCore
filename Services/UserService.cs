using System.Net;
using System.Text.Json;
using WebApi.DataAcesss;
using WebApi.DataTransfer;
using WebApi.Helpers.Exceptions;
using WebApi.Models;

namespace WebApi.Services;

public class UserService {
    private readonly ILogger<UserService> _logger;
    private readonly UserDao _userDao;
    private readonly Database _database;
    private readonly RedisService _redisService;

    public UserService(ILogger<UserService> logger, UserDao userDao, Database database, RedisService redisService) {
        _logger = logger;
        _userDao = userDao;
        _database = database;
        _redisService = redisService;
    }

    public async Task TestTransaction() {
        using var conn = _database.GetConnection();
        await conn.OpenAsync();
        using var transaction = await conn.BeginTransactionAsync();

        try {
            var user = await _userDao.GetUser(44434, transaction);
            user.Password = "test";

            await _userDao.UpdateUser((UserDto)user, transaction);

            await transaction.CommitAsync();
            
        } catch (Exception e) {
            await transaction.RollbackAsync();
            
            throw e;
        }
    }

    public async Task<User> TestRedis() {
        var redis = _redisService.GetDatabase();

        // var user = await _userDao.GetUser(44434);

        // await redis.StringSetAsync("test", JsonSerializer.Serialize(user));

        var test = await redis.StringGetAsync("test");

        if (test.IsNull) throw new HttpException(HttpStatusCode.NotFound, "User not found");

        return JsonSerializer.Deserialize<User>(test);

    }
}