namespace WebApi.Models;

public class User {
    public int IdPersonal { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime FechaAlta { get; set; }
    public DateTime UltimoAcceso { get; set; }
    public UserStatus Estatus { get; set; }
}

public enum UserStatus {
    Active = 1,
    Inactive = 0
}