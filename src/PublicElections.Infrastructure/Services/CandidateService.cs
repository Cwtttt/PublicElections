using Microsoft.EntityFrameworkCore;
using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services
{
    public class CandidateService : BaseService, ICandidateService
    {
        public CandidateService(DataContext context) 
            : base(context) { }

        public async Task<Result> AddAsync(Candidate candidate)
        {
            try
            {
                var exist = await _context.Candidates
                    .AnyAsync(c => c.ElectionId == candidate.ElectionId && c.Name == candidate.Name);

                if (exist)
                    return new Result { Success = false, Errors = new[] { "Candidate already exists" } };

                await _context.AddAsync(candidate);
                await _context.SaveChangesAsync();

                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Result { Success = false, Errors = new[] { ex.ToString() } };
            }
        }

        public async Task<Result> DeleteAsync(int candidateId)
        {
            try
            {
                var candidate = await _context.Candidates
                    .FirstOrDefaultAsync(c => c.Id == candidateId);

                if (candidate == null)
                    return new Result { Success = false };

                _context.Remove(candidate);
                await _context.SaveChangesAsync();

                return new Result { Success = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Result { Success = false, Errors = new[] { ex.ToString() } };
            }
        }

        public async Task<Candidate> GetByIdAsync(int candidateId)
        {
            return await _context.Candidates.FirstOrDefaultAsync(c => c.Id == candidateId);
        }

        public async Task<Result> UpdateAsync(Candidate candidate)
        {
            _context.Candidates.Update(candidate);
            var updated = await _context.SaveChangesAsync();
            return new Result() { Success = updated > 0 };
        }
    }
}
