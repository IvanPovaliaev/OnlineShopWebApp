using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class InFileUsersRepository : IUsersRepository
    {
        public const string FilePath = @".\Data\Users.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<User> _users;

        public InFileUsersRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _users = _jsonRepositoryService.Upload<User>(FilePath);
        }

        public List<User> GetAll() => _users;

        public User Get(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return user;
        }

        public User GetByEmail(string email)
        {
            var user = GetAll().
                        FirstOrDefault(u => u.Email == email);
            return user;
        }

        public void Add(User user)
        {
            _users.Add(user);
            _jsonRepositoryService.SaveChanges(FilePath, _users);
        }

        public void Update(User user)
        {
            var repositoryUser = Get(user.Id);

            if (repositoryUser is null)
            {
                return;
            }

            repositoryUser.Email = user.Email;
            repositoryUser.Password = user.Password;
            repositoryUser.Name = user.Name;
            repositoryUser.Phone = user.Phone;

            _jsonRepositoryService.SaveChanges(FilePath, _users);
        }

        public void Delete(Guid id)
        {
            var user = Get(id);
            _users.Remove(user);
            _jsonRepositoryService.SaveChanges(FilePath, _users);
        }
    }
}
