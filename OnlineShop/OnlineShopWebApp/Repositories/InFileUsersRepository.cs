using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Models;
using OnlineShopWebApp.Services;
using System.Collections.Generic;

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

        public void Add(User user)
        {
            _users.Add(user);
            _jsonRepositoryService.SaveChanges(FilePath, _users);
        }
    }
}
