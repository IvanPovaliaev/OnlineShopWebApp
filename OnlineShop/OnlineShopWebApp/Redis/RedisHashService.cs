using Serilog;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Redis
{
    public class RedisHashService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly SemaphoreSlim _mutex = new(1, 1);

        public RedisHashService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _db = redis.GetDatabase();
        }

        /// <summary>
        /// Caches value with key to target hash table
        /// </summary>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKey">Target field key in hash table</param>
        /// <param name="value">Value to cache</param>
        public async Task SetHashFieldAsync(string hashKey, string fieldKey, string value)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция SetHashFieldAsync пропущена для таблицы {hashKey} с ключом {fieldKey}.");
                return;
            }

            await _mutex.WaitAsync();

            try
            {
                await _db.HashSetAsync(hashKey, fieldKey, value);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка установки значения в Redis для ключа {fieldKey} таблицы {hashKey}");
                return;
            }
            finally
            {
                _mutex.Release();
            }
        }

        /// <summary>
        /// Caches Dictionary to target hash table
        /// </summary>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKeysWithValues">Dictionary witch fielKey-value for hash table</param>
        public async Task SetHashFieldsAsync(string hashKey, Dictionary<string, string> fieldKeysWithValues)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция SetHashFieldsAsync пропущена для таблицы {hashKey}.");
                return;
            }

            await _mutex.WaitAsync();

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
            finally
            {
                _mutex.Release();
            }
        }

        /// <summary>
        /// Get value from hash table by its key
        /// </summary>
        /// <returns>String value</returns>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKey">Target field key in hash table</param>
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

        /// <summary>
        /// Get List of all values from hash table
        /// </summary>
        /// <returns>List of all values (string)</returns>
        /// <param name="hashKey">Target hash table key</param>
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

        /// <summary>
        /// Remove target field from hash table
        /// </summary>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKey">Target field key in hash table</param>
        public async Task RemoveHashFieldAsync(string hashKey, string fieldKey)
        {
            if (!IsRedisAvailable())
            {
                Log.Warning($"Redis недоступен. Операция RemoveHashFieldAsync пропущена для таблицы {hashKey} с ключом {fieldKey}.");
                return;
            }

            await _mutex.WaitAsync();

            try
            {
                await _db.HashDeleteAsync(hashKey, fieldKey);
            }
            catch (RedisConnectionException e)
            {
                Log.Error(e, $"Ошибка удаления значения из Redis для ключа {fieldKey} таблицы {hashKey}");
            }
            finally
            {
                _mutex.Release();
            }
        }

        /// <summary>
        /// Checks the availability of the Redis connection.
        /// </summary>
        /// <returns>Returns true if the connection to Redis is established; otherwise, false.</returns>
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
