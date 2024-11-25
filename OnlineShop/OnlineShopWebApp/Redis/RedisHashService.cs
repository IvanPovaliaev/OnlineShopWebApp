using Serilog;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Redis
{
    public class RedisHashService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;

        public RedisHashService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        public async Task SetHashFieldAsync(string hashKey, string fieldKey, string value)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция SetHashFieldAsync пропущена для таблицы {hashKey} с ключом {fieldKey}.");
                return;
            }

            try
            {
                await _db.HashSetAsync(hashKey, fieldKey, value);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка установки значения в Redis для ключа {fieldKey} таблицы {hashKey}");
                return;
            }
        }

        public async Task SetHashFieldsAsync(string hashKey, Dictionary<string, string> fieldKeysWithValues)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция SetHashFieldsAsync пропущена для таблицы {hashKey}.");
                return;
            }

            try
            {
                var hashEntries = fieldKeysWithValues.Select(kv => new HashEntry(kv.Key, kv.Value))
                                                     .ToArray();
                await _db.HashSetAsync(hashKey, hashEntries);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка установки значения в Redis для таблицы {hashKey}");
                return;
            }
        }

        public async Task<string?> TryGetHashFieldAsync(string hashKey, string fieldKey)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция GetHashFieldAsync пропущена для таблицы {hashKey} с ключом {fieldKey}.");
                return null;
            }

            try
            {
                return await _db.HashGetAsync(hashKey, fieldKey);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка получения значения из Redis для ключа {fieldKey} таблицы {hashKey}");
                return null;
            }
        }

        public async Task<List<string>?> GetAllValuesAsync(string hashKey)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция GetAllValuesAsync пропущена для таблицы {hashKey}.");
                return null;
            }

            try
            {
                var values = await _db.HashValuesAsync(hashKey);
                return values.Select(value => value.ToString())
                             .ToList();
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка полечения значений для таблицы {hashKey}");
                return null;
            }
        }

        public async Task RemoveHashFieldAsync(string hashKey, string fieldKey)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция RemoveHashFieldAsync пропущена для таблицы {hashKey} с ключом {fieldKey}.");
                return;
            }

            try
            {
                await _db.HashDeleteAsync(hashKey, fieldKey);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка удаления значения из Redis для ключа {fieldKey} таблицы {hashKey}");
            }
        }

        private bool IsRedisAvailable()
        {
            if (!_redis.IsConnected)
            {
                Log.Warning("Redis недоступен. Проверьте подключение.");
                return false;
            }
            return true;
        }
    }
}
