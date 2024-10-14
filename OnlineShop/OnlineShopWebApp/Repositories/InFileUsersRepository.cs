using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
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

        public User GetUserByEmail(string email)
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
    }
}
