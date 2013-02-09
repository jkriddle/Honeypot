using System.Threading.Tasks;
using Honeypot.Domain;

namespace Honeypot.Services
{
    public interface INotificationService
    {
        Task<bool> Notify(User recipient, INotificationTemplate template, object messageParams);
    }
}
