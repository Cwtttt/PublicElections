using PublicElections.Domain.Dto;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.Ioc;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services.Interfaces
{
    public interface IElectionService : IScopedService
    {
        Task<Result> CreateElectionAsync(Election election);
        Task<Result> DeleteElectionAsync(int electionId);
    }
}
