using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PublicElections.Domain.Dto;
using PublicElections.Domain.Entities;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Options;
using PublicElections.Infrastructure.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;
        private readonly WebSettings _webSettings;
        private readonly IEmailService _emailService;
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            TokenValidationParameters tokenValidationParameters,
            DataContext context,
            SignInManager<ApplicationUser> signInManager,
            IOptions<WebSettings> webSettings,
            IEmailService emailService)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _signInManager = signInManager;
            _webSettings = webSettings.Value;
            _emailService = emailService;
        }

        public async Task<AuthenticationResult> RegisterAsync(NewUser user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email address already exists" }
                };
            }

            var newUser = new ApplicationUser
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.Email,
                BirthDate = user.BirthDate,
                Email = user.Email
            };

            var createUser = await _userManager.CreateAsync(newUser, user.Password);

            if (!createUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUserAsync(newUser);
        }


        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User doesn't exist" }
                };
            }

            var userHasValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHasValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User and password combination is wrong" }
                };
            }

            //var result = await _signInManager.PasswordSignInAsync(loggedUser.Email, loggedUser.Password, false, lockoutOnFailure: true);
            //if (result.Succeeded)
            //{
            //    return await GenerateAuthenticationResultForUserAsync(user);
            //}
            //else if (result.IsLockedOut)
            //{
            //    return new AuthenticationResult
            //    {
            //        Errors = new[] { "User account locked out." }
            //    };
            //}
            //else
            //{
            //    return new AuthenticationResult
            //    {
            //        Errors = new[] { "User and password combination is wrong." }
            //    };
            //}

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task GenerateEmailConfirmation(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodeCode = WebUtility.UrlEncode(code);

            var link = $"{_webSettings.WebUrl}{_webSettings.ConfirmationEmailPath}?userid={user.Id}&code={encodeCode}";
            link = $"Witaj nowy użytkowniku Employee Messenger, zaloguj sie klikając w <a href=\"{link}\">link</a> aby korzystać z konta, elo!";

            Mail mailRequest = new Mail()
            {
                ToEmail = user.Email,
                Subject = "Potwierdzenie adresu email",
                Body = link
            };

            await _emailService.SendEmailAsync(mailRequest);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Incalid Token" } };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims
                .Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exist" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalided" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been used" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(
                validatedToken.Claims
                .Single(x => x.Type == "id").Value);

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;

                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);

                _tokenValidationParameters.ValidateLifetime = true;

                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }
        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            ApplicationUser appUser = await _userManager.FindByEmailAsync(user.Email);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("id", user.Id)
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                UserId = appUser.Id
            };
        }
    }
}
