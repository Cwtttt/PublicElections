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
using System.Security.Claims;
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
        public IActionResult Add([FromBody] AddVoteRequest request)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.Claims
                .FirstOrDefault(x => x.Type == "id").Value;
            if (!_voteService.CheckIfUserCanVote(userId, request.ElectionId)) return Unauthorized();

            _voteService.Add(userId, request.ElectionId, request.CandidateId);
            return Ok();
        }
    }
}
