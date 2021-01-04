using PublicElections.Domain.Models;
using PublicElections.Infrastructure.Ioc;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services.Interfaces
{
    public interface IIdentityService : IScopedService
    {
        Task<AuthenticationResult> RegisterAsync(NewUser user);
        Task<AuthenticationResult> LoginAsync(string email, string password);
    }
}
