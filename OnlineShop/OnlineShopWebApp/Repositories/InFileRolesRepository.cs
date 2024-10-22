using OnlineShopWebApp.Areas.Admin.Models;
using OnlineShopWebApp.Interfaces;
using OnlineShopWebApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineShopWebApp.Repositories
{
    public class InFileRolesRepository : IRolesRepository
    {
        public const string FilePath = @".\Data\Roles.json";
        private JsonRepositoryService _jsonRepositoryService;
        private List<Role> _roles;

        public InFileRolesRepository(JsonRepositoryService jsonService)
        {
            _jsonRepositoryService = jsonService;
            _roles = _jsonRepositoryService.Upload<Role>(FilePath);
        }

        public List<Role> GetAll() => _roles;

        public Role Get(Guid id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            return role;
        }

        public void Add(Role role)
        {
            _roles.Add(role);
            _jsonRepositoryService.SaveChanges(FilePath, _roles);
        }

        public void Add(List<Role> roles)
        {
            _roles.AddRange(roles);
            _jsonRepositoryService.SaveChanges(FilePath, _roles);
        }

        public void Delete(Guid id)
        {
            var role = Get(id);
            _roles.Remove(role);
            _jsonRepositoryService.SaveChanges(FilePath, _roles);
        }
    }
}
