using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.Ioc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services.Interfaces
{
    public interface IElectionService : IScopedService
    {
        Task<List<Election>> GetAllAsync();
        Task<List<ElectionForUser>> GetAllForUserAsync(string userId);
        Task<Election> GetByIdAsync(int electionId);
        Task<Result> CreateAsync(Election election);
        Task<Result> DeleteAsync(int electionId);
        Task<Result> UpdateAsync(Election election);
        Task<List<Candidate>> GetAllCandidatesAsync(int electionId); 
        Task<ElectionResult> GetElectionResultAsync(int electionId);
    }
}
