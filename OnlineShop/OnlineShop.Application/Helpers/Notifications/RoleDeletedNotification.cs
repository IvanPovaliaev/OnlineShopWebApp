using MediatR;

namespace OnlineShop.Application.Helpers.Notifications
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
