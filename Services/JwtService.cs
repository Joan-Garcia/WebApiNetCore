using WebApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Authentication;
using Microsoft.Extensions.Options;

namespace WebApi.Services;

public class JwtService {

    private readonly JwtOptions _jwtOptions;

    public JwtService(IOptions<JwtOptions> jwtOptions) {
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(User user) {

        var claims = new Claim[] {
            new(JwtRegisteredClaimNames.Sub, user.IdPersonal.ToString()),
            new(JwtRegisteredClaimNames.Name, user.Name)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(8),
            signingCredentials
        );

        string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
}