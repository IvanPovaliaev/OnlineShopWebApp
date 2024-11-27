using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Domain.Interfaces
{
    public interface IRedisCacheService
    {
        /// <summary>
        /// Caches value with key to target hash table
        /// </summary>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKey">Target field key in hash table</param>
        /// <param name="value">Value to cache</param>
        Task SetHashFieldAsync(string hashKey, string fieldKey, string value);

        /// <summary>
        /// Caches Dictionary to target hash table
        /// </summary>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKeysWithValues">Dictionary witch fielKey-value for hash table</param>
        Task SetHashFieldsAsync(string hashKey, Dictionary<string, string> fieldKeysWithValues);

        /// <summary>
        /// Get value from hash table by its key
        /// </summary>
        /// <returns>String value</returns>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKey">Target field key in hash table</param>
        Task<string?> TryGetHashFieldAsync(string hashKey, string fieldKey);

        /// <summary>
        /// Get List of all values from hash table
        /// </summary>
        /// <returns>List of all values (string)</returns>
        /// <param name="hashKey">Target hash table key</param>
        Task<List<string>?> GetAllValuesAsync(string hashKey);

        /// <summary>
        /// Remove target field from hash table
        /// </summary>
        /// <param name="hashKey">Target hash table key</param>
        /// <param name="fieldKey">Target field key in hash table</param>
        Task RemoveHashFieldAsync(string hashKey, string fieldKey);
    }
}