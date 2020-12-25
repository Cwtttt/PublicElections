﻿using Microsoft.EntityFrameworkCore;
using PublicElections.Domain.Dto;
using PublicElections.Domain.Entities;
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
            : base(context)
        {

        }

        public async Task<List<Election>> GetAllAsync()
        {
            return await _context.Elections.ToListAsync();
        }

        public Task<List<Election>> GetByIdAsync(int electionId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> CreateElectionAsync(Election election)
        {
            try
            {
                await _context.AddAsync(election);
                await _context.SaveChangesAsync();
                return new Result() { Success = true };
            }
            catch(Exception ex)
            {
                return new Result() { Success = false, Errors = new[] { ex.ToString() } };
            }
        }

        public async Task<Result> DeleteElectionAsync(int electionId)
        {
            try
            {
                var election = _context.Elections
                    .Include(e => e.Candidates)
                    .FirstOrDefault(e => e.Id == electionId);

                if(election == null) 
                    return new Result() { Success = false, Errors = new[] { "Election does not exist" } };

                _context.Elections.Remove(election);
                await _context.SaveChangesAsync();

                return new Result() { Success = true };
            }
            catch (Exception ex)
            {
                return new Result() { Success = false, Errors = new[] { ex.ToString() } };
            }
        }

        //CANDIDATES
        public async Task<List<Candidate>> GetAllElectionCandidatesAsync(int electionId)
        {
            return await _context.Candidates
                .Where(c => c.ElectionId == electionId)
                .ToListAsync();
        }
    }
}
