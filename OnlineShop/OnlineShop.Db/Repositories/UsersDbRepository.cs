using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<List<User>> GetAllAsync() => await _databaseContext.Users.Include(u => u.Role)
                                                                                   .ToListAsync();

        public async Task<User> GetAsync(Guid id)
        {
            return await _databaseContext.Users.Include(u => u.Role)
                                               .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _databaseContext.Users.Include(u => u.Role)
                                               .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            await _databaseContext.Users.AddAsync(user);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var repositoryUser = await GetAsync(user.Id);

            if (repositoryUser is null)
            {
                return;
            }

            repositoryUser.Email = user.Email;
            repositoryUser.Password = user.Password;
            repositoryUser.Name = user.Name;
            repositoryUser.Phone = user.Phone;
            repositoryUser.Role = user.Role;

            await _databaseContext.SaveChangesAsync();
        }

        public async Task ChangeRolesToUserAsync(Guid oldRoleId, Guid userRoleId)
        {
            var userRole = await _databaseContext.Roles.FirstOrDefaultAsync(r => r.Id == userRoleId)!;

            var users = await _databaseContext.Users.Where(user => user.Role.Id == oldRoleId)
                                              .ToArrayAsync();

            foreach (var user in users)
            {
                user.Role = userRole;
            }

            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetAsync(id);
            _databaseContext.Users.Remove(user);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
