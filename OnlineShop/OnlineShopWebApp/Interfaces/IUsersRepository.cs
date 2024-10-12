using OnlineShopWebApp.Models;
using System.Collections.Generic;

namespace OnlineShopWebApp.Interfaces
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        List<User> GetAll();

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user">Target user</param>
        void Add(User user);
    }
}
