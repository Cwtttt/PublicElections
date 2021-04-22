using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PublicElections.Api.Controllers.V1.Abstract;
using PublicElections.Contracts.Requests.Identity;
using PublicElections.Contracts.Response.Identity;
using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace PublicElections.Api.Controllers.V1
{
    public class IdentityController : ApiControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public IdentityController(
            IIdentityService identityService,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _identityService = identityService;
            _userManager = userManager;
            _mapper = mapper;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            NewUser newUser = _mapper.Map<NewUser>(request);
            var authResponse = await _identityService.RegisterAsync(newUser);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityService.LoginAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }

        [HttpPost("verifyemail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest verifyEmailRequest)
        {
            var user = await _userManager.FindByIdAsync(verifyEmailRequest.UserId);
            if (user == null)
            {
                return BadRequest(new { Error = "User doesn't exist." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, verifyEmailRequest.Code);
            if (!result.Succeeded)
            {
                return BadRequest(new { Error = "Error during confirmation user email." });
            }

            return Ok(new { Message = "User email confirmated successfull" });
        }
    }
}
