using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class UsersDbRepository : IUsersRepository
    {
        private readonly DatabaseContext _databaseContext;

        public UsersDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<User>> GetAllAsync() => [];

        public async Task<User> GetAsync(Guid id)
        {
            return new User();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return new User();
        }

        public async Task AddAsync(User user)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task ChangeRolesToUserAsync(Guid oldRoleId, Guid userRoleId)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
