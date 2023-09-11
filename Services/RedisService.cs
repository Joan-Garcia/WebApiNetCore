using StackExchange.Redis;

namespace WebApi.Services;

public class RedisService {
    private Lazy<ConnectionMultiplexer> _lazyConnection;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly ILogger<RedisService> _logger;

    public RedisService(IConfiguration configuration, ILogger<RedisService> logger) {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("RedisConnection");
        _logger = logger;

        _lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
            return ConnectionMultiplexer.Connect(_connectionString);
        });

        EventHandlers();

    }

    public IDatabase GetDatabase() {
        return _lazyConnection.Value.GetDatabase();
    }

    private void EventHandlers() {
        _lazyConnection.Value.ConnectionFailed += (_, e) => {
            _logger.LogError("REDIS: Connection failed.");
        };

        _lazyConnection.Value.ConnectionRestored += (_, e) => {
            _logger.LogInformation("REDIS: Connection restored.");
        };

        _lazyConnection.Value.ErrorMessage += (_, e) => {
            _logger.LogError("REDIS: Error => {@Message}", e.Message);
        };

        _lazyConnection.Value.InternalError += (_, e) => {
            _logger.LogError("REDIS: Internal error => {@Message}", e.Exception.Message);
        };
    }
}