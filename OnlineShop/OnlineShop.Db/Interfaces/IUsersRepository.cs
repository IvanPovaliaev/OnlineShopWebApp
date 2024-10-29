using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;

namespace OnlineShop.Db.Interfaces
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        List<User> GetAll();

        /// <summary>
        /// Get user by GUID
        /// </summary>
        /// <returns>User; returns null if user not found</returns>
        /// <param name="id">User Id (GUID)</param>
        User Get(Guid id);

        /// <summary>
        /// Get user by email
        /// </summary>
        /// <returns>User; returns null if user not found</returns>
        /// <param name="email">Target email</param>
        User GetByEmail(string email);

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user">Target user</param>
        void Add(User user);

        /// <summary>
        /// Update user with identical id. If user is not in the repository - does nothing.
        /// </summary>
        /// <param name="user">Target user</param>
        void Update(User user);

        /// <summary>
        /// Change roles by is id to User role for every user in reposutory.
        /// </summary>
        /// <param name="oldRoleId">Old role id</param>
        /// <param name="userRoleId">User role id</param>
        void ChangeRolesToUser(Guid oldRoleId, Guid userRoleId);

        /// <summary>
        /// Delete user by GUID
        /// </summary>
        /// <param name="id">User Id (GUID)</param>
        void Delete(Guid id);
    }
}
