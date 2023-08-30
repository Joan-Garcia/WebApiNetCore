using WebApi.DataAcesss;
using WebApi.DataTransfer;

namespace WebApi.Services;

public class UserService {
    private readonly ILogger<UserService> _logger;
    private readonly UserDao _userDao;
    private readonly Database _database;

    public UserService(ILogger<UserService> logger, UserDao userDao, Database database) {
        _logger = logger;
        _userDao = userDao;
        _database = database;
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
}