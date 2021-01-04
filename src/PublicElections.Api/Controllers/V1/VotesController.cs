using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PublicElections.Api.Controllers.V1.Abstract;
using PublicElections.Contracts.Requests.Vote;
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
        public VotesController()
        {

        }

        [HttpPost()]
        public async Task<IActionResult> Add([FromBody] AddVoteRequest request)
        {

            return Ok();
        }
    }
}
