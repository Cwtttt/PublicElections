using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicElections.Api.Controllers.V1.Abstract;
using PublicElections.Contracts.Requests.Candidate;
using PublicElections.Contracts.Requests.Election;
using PublicElections.Contracts.Response.Candidates;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.Services.Interfaces;
using System.Threading.Tasks;

namespace PublicElections.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class CandidatesController : ApiControllerBase
    {
        private readonly ICandidateService _candidateService;
        private readonly IMapper _mapper;

        public CandidatesController(
            ICandidateService candidateService,
            IMapper mapper)
        {
            _candidateService = candidateService;
            _mapper = mapper;
        }

        [HttpGet("{candidateId}")]
        public async Task<IActionResult> Get([FromRoute] int candidateId)
        {
            var candidate = await _candidateService.GetByIdAsync(candidateId);
            var candidateResponse = _mapper.Map<CandidateResponse>(candidate);

            return Ok(candidateResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddCandidateRequest request)
        {
            Candidate candidate = new Candidate()
            {
                Name = request.Name,
                ElectionId = request.ElectionId
            };

            var result = await _candidateService.AddAsync(candidate);

            if (!result.Success)
                return StatusCode(StatusCodes.Status409Conflict, result.Errors);

            return Ok(new { Message = "Candidate added successfully" });
        }

        [HttpDelete("{candidateId}")]
        public async Task<IActionResult> Delete([FromRoute] int candidateId)
        {
            var result = await _candidateService.DeleteAsync(candidateId);

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

            return NoContent();
        }

        [HttpPut("{candidateId}")]
        public async Task<IActionResult> Update([FromRoute] int candidateId, [FromBody] UpdateCandidateRequest request)
        {
            var candidate = await _candidateService.GetByIdAsync(request.CandidateId);

            candidate.Name = request.Name;

            var result = await _candidateService.UpdateAsync(candidate);

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

            return Ok(new { Message = "Candidate updated successfully" });
        }
    }
}
