using Serilog;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Redis
{
	public class RedisService
	{
		private readonly IConnectionMultiplexer _redis;
		private readonly IDatabase _db;
		private readonly SemaphoreSlim _mutex = new(1, 1);

		public RedisService(IConnectionMultiplexer redis)
		{
			_redis = redis;
			_db = redis.GetDatabase();
		}

		public async Task SetAsync(string key, string value)
		{
			if (!IsRedisAvailable())
			{
				Log.Warning($"Redis недоступен. Операция SetAsync пропущена для ключа {key}.");
				return;
			}

			await _mutex.WaitAsync();

			try
			{
				await _db.StringSetAsync(key, value);
			}
			catch (RedisConnectionException e)
			{
				Log.Error(e, $"Ошибка установки значения в Redis для ключа {key}.");
				return;
			}
			finally
			{
				_mutex.Release();
			}
		}

		public async Task<string?> TryGetAsync(string key)
		{
			if (!IsRedisAvailable())
			{
				Log.Warning($"Redis недоступен. Операция GetAsync пропущена для для ключа {key}.");
				return null;
			}

			try
			{
				return await _db.StringGetAsync(key);
			}
			catch (RedisConnectionException e)
			{
				Log.Error(e, $"Ошибка получения значения из Redis для ключа {key}");
				return null;
			}
		}

		public async Task RemoveAsync(string key)
		{
			if (!IsRedisAvailable())
			{
				Log.Warning($"Redis недоступен. Операция RemoveAsync пропущена для ключа {key}.");
				return;
			}

			await _mutex.WaitAsync();

			try
			{
				await _db.KeyDeleteAsync(key);
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
