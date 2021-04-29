using Microsoft.EntityFrameworkCore;
using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services
{
    public class ElectionService : BaseService, IElectionService
    {
        public ElectionService(DataContext context) 
            : base(context) { }

        public async Task<List<Election>> GetAllAsync()
        {
            return await _context.Elections.ToListAsync();
        }

        public async Task<Election> GetByIdAsync(int electionId)
        {
            return await _context.Elections.FirstOrDefaultAsync(e => e.Id == electionId);
        }

        public async Task<Result> CreateAsync(Election election)
        {
            try
            {
                await _context.AddAsync(election);
                await _context.SaveChangesAsync();
                return new Result() { Success = true };
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Result() { Success = false, Errors = new[] { ex.ToString() } };
            }
        }
        public async Task<Result> UpdateAsync(Election election)
        {
            _context.Elections.Update(election);
            var updated = await _context.SaveChangesAsync();
            return new Result() { Success = updated > 0 };
        }

        public async Task<Result> DeleteAsync(int electionId)
        {
            try
            {
                var election = _context.Elections
                    .Include(e => e.Candidates)
                    .FirstOrDefault(e => e.Id == electionId);

                if(election == null)
                    return new Result() { Success = true };

                _context.Elections.Remove(election);
                await _context.SaveChangesAsync();

                return new Result() { Success = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new Result() { Success = false, Errors = new[] { ex.ToString() } };
            }
        }

        //CANDIDATES
        public async Task<List<Candidate>> GetAllCandidatesAsync(int electionId)
        {
            return await _context.Candidates
                .Where(c => c.ElectionId == electionId)
                .ToListAsync();
        }

        public async Task<List<ElectionForUser>> GetAllForUserAsync(string userId)
        {
            var elections = await _context.Elections.ToListAsync();

            var participations = await _context.Participations
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return elections.Select(x => new ElectionForUser()
            {
                Id = x.Id,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                CanParticipate = !participations.Any(p => p.ElectionId == x.Id)
            }).ToList();
        }

        //RESULTS
        public async Task<ElectionResult> GetElectionResultAsync(int electionId)
        {
            ElectionResult electionResult = new ElectionResult();

            var votes = await _context.Votes
                .Where(v => v.ElectionId == electionId)
                .ToListAsync();

            electionResult.VotesAmount = votes.Count;

            var candidates = await _context.Candidates
                .Where(c => c.ElectionId == electionId)
                .ToListAsync();

            foreach(var candidate in candidates)
            {
                CandidateVotesResult candidateResult = new CandidateVotesResult();

                candidateResult.CandidateName = candidate.Name;
                candidateResult.Percentages = votes.GetPercentageOfVotes(candidate.Id);
                candidateResult.VotesCount = votes.Where(v => v.CandidateId == candidate.Id).Count();

                electionResult.CandidatesResults.Add(candidateResult);
            }

            return electionResult;
        }
    }
    public static class VotesExtension
    {
        public static decimal GetPercentageOfVotes(this List<Vote> source, int candidateId)
        {
            if (!(source.Count > 0)) return 0.0m;

            return Math.Round(100.00m * (decimal)source.Where(v => v.CandidateId == candidateId).Count() / (decimal)source.Count, 3);
        }
    }
}
