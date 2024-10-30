using MediatR;
using OnlineShopWebApp.Helpers.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Helpers.Handlers
{
    public class UsersOnRoleDeletedHandler(AccountsService accountsService) : INotificationHandler<RoleDeletedNotification>
    {
        private readonly AccountsService _accountsService = accountsService;

        public async Task Handle(RoleDeletedNotification notification, CancellationToken cancellationToken)
        {
            await _accountsService.ChangeRolesToUserAsync(notification.RoleId);
        }
    }
}
