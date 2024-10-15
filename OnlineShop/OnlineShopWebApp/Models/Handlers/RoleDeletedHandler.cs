using MediatR;
using OnlineShopWebApp.Models.Notifications;
using OnlineShopWebApp.Services;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Models.Handlers
{
    public class RoleDeletedHandler : INotificationHandler<RoleDeletedNotification>
    {
        private readonly AccountsService _accountsService;

        public RoleDeletedHandler(AccountsService accountsService)
        {
            _accountsService = accountsService;
        }

        public async Task Handle(RoleDeletedNotification notification, CancellationToken cancellationToken)
        {
            _accountsService.ChangeRolesToUser(notification.RoleId);
        }
    }
}
