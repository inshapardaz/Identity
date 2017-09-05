using System.Threading.Tasks;

namespace Inshapardaz.Identity.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
