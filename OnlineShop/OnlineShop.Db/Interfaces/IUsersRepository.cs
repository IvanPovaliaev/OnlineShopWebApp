using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Interfaces
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        Task<List<User>> GetAllAsync();

        /// <summary>
        /// Get user by GUID
        /// </summary>
        /// <returns>User; returns null if user not found</returns>
        /// <param name="id">User Id (GUID)</param>
        Task<User> GetAsync(Guid id);

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user">Target user</param>
        Task AddAsync(User user);

        /// <summary>
        /// Update user with identical id. If user is not in the repository - does nothing.
        /// </summary>
        /// <param name="user">Target user</param>
        Task UpdateAsync(User user);

        /// <summary>
        /// Change roles by is id to User role for every user in reposutory.
        /// </summary>
        /// <param name="oldRoleId">Old role id</param>
        /// <param name="userRoleId">User role id</param>
        Task ChangeRolesToUserAsync(Guid oldRoleId, Guid userRoleId);

        /// <summary>
        /// Delete user by GUID
        /// </summary>
        /// <param name="id">User Id (GUID)</param>
        Task DeleteAsync(Guid id);
    }
}
