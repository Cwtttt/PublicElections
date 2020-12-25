using PublicElections.Domain.Dto;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.Ioc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services.Interfaces
{
    public interface IElectionService : IScopedService
    {
        Task<List<Election>> GetAllAsync();
        Task<List<Election>> GetByIdAsync(int electionId);
        Task<Result> CreateElectionAsync(Election election);
        Task<Result> DeleteElectionAsync(int electionId);
        Task<List<Candidate>> GetAllElectionCandidatesAsync(int electionId);
    }
}
