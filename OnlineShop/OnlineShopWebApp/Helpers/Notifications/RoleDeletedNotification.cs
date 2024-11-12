using MediatR;

namespace OnlineShopWebApp.Helpers.Notifications
{
    public class RoleDeletedNotification : INotification
    {
        public string RoleName { get; }

        public RoleDeletedNotification(string roleName)
        {
            RoleName = roleName;
        }
    }
}
