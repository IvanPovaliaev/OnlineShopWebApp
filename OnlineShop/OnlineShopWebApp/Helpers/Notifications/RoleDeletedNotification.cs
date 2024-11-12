using MediatR;

namespace OnlineShopWebApp.Helpers.Notifications
{
    public class RoleDeletedNotification : INotification
    {
        public string RoleId { get; }

        public RoleDeletedNotification(string roleId)
        {
            RoleId = roleId;
        }
    }
}
