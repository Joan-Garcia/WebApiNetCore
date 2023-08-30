using WebApi.Models;

namespace WebApi.DataTransfer;

public class UserDto {
    public int IdPersonal { get; set; }
    public string Password { get; set; } = string.Empty;
    public int Estatus { get; set; }

    public static explicit operator UserDto(User v) {
        return new UserDto {
            IdPersonal = v.IdPersonal,
            Password = v.Password,
            Estatus = (int)v.Estatus
        };
    }
}