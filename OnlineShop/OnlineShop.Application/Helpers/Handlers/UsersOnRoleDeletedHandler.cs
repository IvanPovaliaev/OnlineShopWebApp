using MediatR;
using OnlineShop.Application.Helpers.Notifications;
using OnlineShop.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShop.Application.Helpers.Handlers
{
    public class UsersOnRoleDeletedHandler(IAccountsService accountsService) : INotificationHandler<RoleDeletedNotification>
    {
        private readonly IAccountsService _accountsService = accountsService;

        public async Task Handle(RoleDeletedNotification notification, CancellationToken cancellationToken)
        {
            await _accountsService.ChangeRolesToUserAsync(notification.RoleName);
        }
    }
}
