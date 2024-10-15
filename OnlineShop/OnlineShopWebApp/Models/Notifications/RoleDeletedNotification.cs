using MediatR;
using System;

namespace OnlineShopWebApp.Models.Notifications
{
    public class RoleDeletedNotification : INotification
    {
        public Guid RoleId { get; }

        public RoleDeletedNotification(Guid roleId)
        {
            RoleId = roleId;
        }
    }
}
