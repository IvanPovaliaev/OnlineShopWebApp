using Microsoft.EntityFrameworkCore;
using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShop.Db.Repositories
{
    public class RolesDbRepository : IRolesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public RolesDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public async Task<List<Role>> GetAllAsync() => await _databaseContext.Roles.ToListAsync();

        public async Task<Role> GetAsync(Guid id)
        {
            return await _databaseContext.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Role role)
        {
            await _databaseContext.Roles.AddAsync(role);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Role> roles)
        {
            await _databaseContext.Roles.AddRangeAsync(roles);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await GetAsync(id);
            _databaseContext.Roles.Remove(role);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
