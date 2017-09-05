using System.Threading.Tasks;

namespace Inshapardaz.Identity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
