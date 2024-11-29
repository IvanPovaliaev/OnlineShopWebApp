using System.Threading.Tasks;

namespace OnlineShop.Domain.Interfaces
{
    public interface IMailService
    {
        /// <summary>
        /// Send message to target email
        /// </summary>
        /// <param name="email">E-mail address</param>
        /// <param name="subject">Messages subject</param>
        /// <param name="message">Message</param>
        Task SendEmailAsync(string email, string subject, string message);
    }
}
