using Serilog;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Redis
{
    public class RedisCacheService(IConnectionMultiplexer redis)
    {
        private readonly IConnectionMultiplexer _redis = redis;
        private readonly SemaphoreSlim _mutex = new(1, 1);

        public async Task SetAsync(string key, string value)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция Set пропущена для ключа {key}.");
                return;
            }

            await _mutex.WaitAsync();

            try
            {
                var db = _redis.GetDatabase();
                await db.StringSetAsync(key, value);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка установки значения в Redis для ключа {key}");
            }
            finally
            {
                _mutex.Release();
            }
        }

        public async Task<string> TryGetAsync(string key)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция Get пропущена для ключа {key}.");
                return null!;
            }

            try
            {
                var db = _redis.GetDatabase();
                return await db.StringGetAsync(key);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка получения значения из Redis для ключа {key}");
                return null!;
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция Remove пропущена для ключа {key}.");
                return;
            }

            await _mutex.WaitAsync();

            try
            {
                var db = _redis.GetDatabase();
                await db.KeyDeleteAsync(key);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка удаления значения из Redis для ключа {key}");
            }
            finally
            {
                _mutex.Release();
            }
        }

        private bool IsRedisAvailable()
        {
            if (!_redis.IsConnected)
            {
                Log.Warning("Redis недоступен. Проверяйте подключение.");
                return false;
            }
            return true;
        }
    }

}
