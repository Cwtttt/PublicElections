using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.Ioc;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services.Interfaces
{
    public interface ICandidateService : IScopedService
    {
        Task<Result> AddAsync(Candidate candidate);
        Task<Candidate> GetByIdAsync(int candidateId);
        Task<Result> DeleteAsync(int candidateId);
        Task<Result> UpdateAsync(Candidate candidate);
    }
}
