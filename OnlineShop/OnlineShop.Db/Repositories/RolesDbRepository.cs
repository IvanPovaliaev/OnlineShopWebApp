using OnlineShop.Db.Interfaces;
using OnlineShop.Db.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShop.Db.Repositories
{
    public class RolesDbRepository : IRolesRepository
    {
        private readonly DatabaseContext _databaseContext;

        public RolesDbRepository(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public List<Role> GetAll() => _databaseContext.Roles.ToList();

        public Role Get(Guid id)
        {
            return _databaseContext.Roles.FirstOrDefault(r => r.Id == id)!;
        }

        public void Add(Role role)
        {
            _databaseContext.Roles.Add(role);
            _databaseContext.SaveChanges();
        }

        public void AddRange(List<Role> roles)
        {
            _databaseContext.Roles.AddRange(roles);
            _databaseContext.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var role = Get(id);
            _databaseContext.Roles.Remove(role);
            _databaseContext.SaveChanges();
        }
    }
}
