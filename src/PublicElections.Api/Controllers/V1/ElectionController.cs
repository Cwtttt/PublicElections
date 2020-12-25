using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PublicElections.Api.Controllers.V1.Abstract;
using PublicElections.Contracts.Requests.Election;
using PublicElections.Contracts.Response.Candidates;
using PublicElections.Contracts.Response.Elections;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PublicElections.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class ElectionController : ApiControllerBase
    {
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;
        public ElectionController(
            IElectionService electionService, 
            IMapper mapper)
        {
            _electionService = electionService;
            _mapper = mapper;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new { 
                    Errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var elections = await _electionService.GetAllAsync();
            var electionsResponse = _mapper.Map<List<ElectionResponse>>(elections);
            return Ok(electionsResponse);
        }

        [HttpGet("{electionId}")]
        public async Task<IActionResult> GetbyId(int electionId)
        {
            var elections = await _electionService.GetAllAsync();
            var electionsResponse = _mapper.Map<List<ElectionResponse>>(elections);
            return Ok(electionsResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CreateElection([FromBody] CreateElectionRequest request)
        {
            Election election = new Election()
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreateDate = DateTime.Now
            };

            var result = await _electionService.CreateElectionAsync(election);

            if (!result.Success) 
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors );

            return Ok(new { Message = "New election created successfully" });
        }

        [HttpDelete("{electionId}")]
        public async Task<IActionResult> DeleteElection(int electionId)
        {
            var result = await _electionService.DeleteElectionAsync(electionId);

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

            return Ok(new { Message = "Election deleted successfully" });
        }

        //Candidates

        [HttpGet("{electionId}/candidates")]
        public async Task<IActionResult> GetAllElectionCandidates(int electionId)
        {
            var candidates = await _electionService.GetAllElectionCandidatesAsync(electionId);
            var candidatesResponse = _mapper.Map<List<CandidateResponse>>(candidates);
            return Ok(candidatesResponse);
        }
    }
}
