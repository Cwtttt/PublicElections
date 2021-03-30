using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PublicElections.Api.Controllers.V1.Abstract;
using PublicElections.Contracts.Requests.Vote;
using PublicElections.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PublicElections.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class VotesController : ApiControllerBase
    {
        private readonly IVoteService _voteService;
        public VotesController(
            IVoteService voteService)
        {
            _voteService = voteService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddVoteRequest request)
        {
            _voteService.Add(request.UserId, request.ElectionId, request.CandidateId);
            return Ok();
        }
    }
}
