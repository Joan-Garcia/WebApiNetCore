using MySqlConnector;

namespace WebApi.DataAcesss;

public class Database {
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public Database(IConfiguration configuration) {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public MySqlConnection GetConnection() {
        return new MySqlConnection(_connectionString);
    }
}