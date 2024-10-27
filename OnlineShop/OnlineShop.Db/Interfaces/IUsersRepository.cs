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
        /// Change roles of every user in target collection to User.
        /// </summary>
        /// <param name="users">Target users collection</param>
        void ChangeRolesToUser(IEnumerable<User> users);

        /// <summary>
        /// Delete user by GUID
        /// </summary>
        /// <param name="id">User Id (GUID)</param>
        void Delete(Guid id);
    }
}
