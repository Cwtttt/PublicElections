using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicElections.Api.Controllers.V1.Abstract;
using PublicElections.Contracts.Requests.Election;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PublicElections.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class ElectionController : ApiControllerBase
    {
        private readonly IElectionService _electionService;
        public ElectionController(IElectionService electionService)
        {
            _electionService = electionService;
        }

        [HttpPost("api/v1/election")]
        public async Task<IActionResult> CreateElection([FromBody] CreateElectionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(new {Message = "Model state is not valid"});

            Election election = new Election()
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreateDate = DateTime.Now
            };

            var result = await _electionService.CreateElectionAsync(election);

            if (!result.Success) 
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error during creating new election" });

            return Ok(new { Message = "New election created successfully" });
        }

        [HttpDelete("api/v1/election/{electionId}")]
        public async Task<IActionResult> DeleteElection([FromRoute] int electionId)
        {
            if (!ModelState.IsValid) return BadRequest(new { Message = "Model state is not valid" });

            var result = await _electionService.DeleteElectionAsync(electionId);

            return Ok();
        }
    }
}
