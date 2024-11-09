using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class UsersDbRepository : IUsersRepository
    {
        private readonly IdentityContext _identityContext;

        public UsersDbRepository(IdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _identityContext.Users.ToListAsync();
        }

        public async Task<User> GetAsync(Guid id)
        {
            var identityId = id.ToString();
            return await _identityContext.Users.FirstOrDefaultAsync(u => u.Id == identityId);
        }

        public async Task AddAsync(User user)
        {
            await _identityContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await _identityContext.SaveChangesAsync();
        }

        public async Task ChangeRolesToUserAsync(Guid oldRoleId, Guid userRoleId)
        {
            await _identityContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _identityContext.SaveChangesAsync();
        }
    }
}
