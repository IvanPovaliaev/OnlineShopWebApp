using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Db.Repositories
{
    public class UsersDbRepository : IUsersRepository
    {
        private readonly DatabaseContext _databaseContext;

        public UsersDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<User> GetAll() => _databaseContext.Users.ToList();

        public User Get(Guid id)
        {
            return _databaseContext.Users.FirstOrDefault(u => u.Id == id)!;
        }

        public User GetByEmail(string email)
        {
            return _databaseContext.Users.FirstOrDefault(u => u.Email == email)!;
        }

        public void Add(User user)
        {
            _databaseContext.Users.Add(user);
            _databaseContext.SaveChanges();
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
            repositoryUser.Role = user.Role;

            _databaseContext.SaveChanges();
        }

        public void ChangeRolesToUser(Guid oldRoleId, Guid userRoleId)
        {
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId)!;

            var users = _databaseContext.Users.Where(user => user.Role.Id == oldRoleId);

            foreach (var user in users)
            {
                user.Role = userRole;
            }

            _databaseContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var user = Get(id);
            _databaseContext.Users.Remove(user);
            _databaseContext.SaveChanges();
        }
    }
}
