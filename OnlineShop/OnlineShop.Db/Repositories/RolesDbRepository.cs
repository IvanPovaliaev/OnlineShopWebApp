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

        public async Task<List<Role>> GetAllAsync() => [];

        public async Task<Role> GetAsync(Guid id)
        {
            return new Role();
        }

        public async Task AddAsync(Role role)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Role> roles)
        {
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _databaseContext.SaveChangesAsync();
        }
    }
}
