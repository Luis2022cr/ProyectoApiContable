using StackExchange.Redis;

namespace ProyectoApiContable.Services
{
    public class RedisServices: IRedisServices
    {
        private readonly IDatabase _redisDb;

        public RedisServices(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task AgregarLogARedis(string logMessage)
        {
                // Agregar el log a una lista en Redis
                await _redisDb.ListLeftPushAsync("logsApiContable", logMessage);
        }
    }
}
