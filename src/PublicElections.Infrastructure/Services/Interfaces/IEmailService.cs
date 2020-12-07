using PublicElections.Domain.Dto;
using PublicElections.Infrastructure.Ioc;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services.Interfaces
{
    public interface IEmailService : IScopedService
    {
        Task SendEmailAsync(Mail mailRequest);
    }
}
