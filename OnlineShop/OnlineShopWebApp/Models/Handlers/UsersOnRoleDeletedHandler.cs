using MediatR;
using OnlineShopWebApp.Models.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Models.Handlers
{
    public class UsersOnRoleDeletedHandler(AccountsService accountsService) : INotificationHandler<RoleDeletedNotification>
    {
        private readonly AccountsService _accountsService = accountsService;

        public async Task Handle(RoleDeletedNotification notification, CancellationToken cancellationToken)
        {
            _accountsService.ChangeRolesToUser(notification.RoleId);
        }
    }
}
