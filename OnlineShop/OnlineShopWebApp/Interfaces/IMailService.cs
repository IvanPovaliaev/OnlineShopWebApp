using System.Threading.Tasks;

namespace OnlineShopWebApp.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
