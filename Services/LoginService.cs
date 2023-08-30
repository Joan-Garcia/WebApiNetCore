using System.Net;
using WebApi.DataAcesss;
using WebApi.DataTransfer;
using WebApi.Helpers.Exceptions;

namespace WebApi.Services;

public class LoginService {
    private readonly UserDao _userDao;
    private readonly JwtService _jwtService;

    public LoginService(UserDao userDao, JwtService jwtService) {
        _userDao = userDao;
        _jwtService = jwtService;
    }

    public async Task<string> Login(LoginRequestDto request) {
        var userDb = await _userDao.GetUser(request.IdPersonal);

        if (userDb is null) {
            throw new HttpException(HttpStatusCode.NotFound, "User not found");
        }

        if (userDb.Password != request.Password) {
            throw new HttpException(HttpStatusCode.BadRequest, "Invalid password");
        }

        string token = _jwtService.GenerateToken(userDb);

        return token;
    }
}