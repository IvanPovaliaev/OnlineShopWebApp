using System;

namespace OnlineShopWebApp.Services
{
    public class RolesEventService
    {
        public event Action<Guid> RoleDeleted;

        /// <summary>
        /// Initialize RoleDeleted event with roleId
        /// </summary> 
        /// <param name="roleId">Target roleId (GUID)</param>
        public void OnRoleDeleted(Guid roleId)
        {
            RoleDeleted?.Invoke(roleId);
        }
    }
}
