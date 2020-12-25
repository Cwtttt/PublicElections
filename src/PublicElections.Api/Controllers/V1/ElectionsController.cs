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
    public class ElectionsController : ApiControllerBase
    {
        private readonly IElectionService _electionService;
        private readonly IMapper _mapper;
        public ElectionsController(
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
            var election = await _electionService.GetByIdAsync(electionId);
            var electionResponse = _mapper.Map<List<ElectionResponse>>(election);
            return Ok(electionResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateElectionRequest request)
        {
            Election election = new Election()
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreateDate = DateTime.Now
            };

            var result = await _electionService.CreateAsync(election);

            if (!result.Success) 
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors );

            return Ok(new { Message = "New election created successfully" });
        }

        [HttpDelete("{electionId}")]
        public async Task<IActionResult> Delete(int electionId)
        {
            var result = await _electionService.DeleteAsync(electionId);

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

            return Ok(new { Message = "Election deleted successfully" });
        }

        [HttpPut("electionId")]
        public async Task<IActionResult> Update([FromRoute] int electionId, [FromBody] UpdateElectionRequest request)
        {
            var election = await _electionService.GetByIdAsync(electionId);

            election.Name = request.Name;
            election.StartDate = request.StartDate;
            election.EndDate = request.EndDate;

            var result = await _electionService.UpdateAsync(election);

            if (!result.Success)
                return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);

            return Ok(new { Message = "Election deleted successfully" });
        }

        //Candidates

        [HttpGet("{electionId}/candidates")]
        public async Task<IActionResult> GetAllCandidates(int electionId)
        {
            var candidates = await _electionService.GetAllCandidatesAsync(electionId);
            var candidatesResponse = _mapper.Map<List<CandidateResponse>>(candidates);
            return Ok(candidatesResponse);
        }
    }
}
