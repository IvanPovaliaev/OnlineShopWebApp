﻿using OnlineShopWebApp.Models;
using System;
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
        /// Delete user by GUID
        /// </summary>
        /// <param name="id">User Id (GUID)</param>
        void Delete(Guid id);
    }
}
