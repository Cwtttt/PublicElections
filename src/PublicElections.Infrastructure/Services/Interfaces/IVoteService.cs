using PublicElections.Domain.Models;
using PublicElections.Infrastructure.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services.Interfaces
{
    public interface IVoteService : IScopedService
    {
        Task<Result> Add(string userId, int electionId, int candidateId);
    }
}
