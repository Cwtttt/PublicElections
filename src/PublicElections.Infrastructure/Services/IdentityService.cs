using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PublicElections.Domain.Entities;
using PublicElections.Domain.Models;
using PublicElections.Infrastructure.EntityFramework;
using PublicElections.Infrastructure.Services.Interfaces;
using PublicElections.Infrastructure.Settings;
using System;
using System.Collections.Generic;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly DataContext _context;
        private readonly WebSettings _webSettings;
        private readonly IEmailService _emailService;
        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IOptions<JwtSettings> jwtSettings,
            DataContext context,
            SignInManager<ApplicationUser> signInManager,
            IOptions<WebSettings> webSettings,
            IEmailService emailService, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _signInManager = signInManager;
            _webSettings = webSettings.Value;
            _emailService = emailService;
            _roleManager = roleManager;
        }

        public async Task<AuthenticationResult> RegisterAsync(NewUser user)
        {
            bool userExist = await CheckIfUserExistAsync(user.Pesel);

            if (userExist)
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
                Email = user.Email
            };

            var password = GenerateRandomPassword();

            var createUser = await _userManager.CreateAsync(newUser, password);

            if (!createUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createUser.Errors.Select(x => x.Description)
                };
            }

            var sendConfirmationEmail = await GenerateEmailConfirmation(newUser.Email, password);

            if (!sendConfirmationEmail)
            {
                await _userManager.DeleteAsync(newUser);
                return new AuthenticationResult
                {
                    Errors = new[] {"Send confirmation email fail"}
                };
            }

            return new AuthenticationResult
            {
                Success = true
            };
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

            var result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return await GenerateAuthenticationResultForUserAsync(user);
            }
            else if (result.IsLockedOut)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User account locked out." }
                };
            }
            else
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User and password combination is wrong." }
                };
            }
        }

        private async Task<bool> GenerateEmailConfirmation(string userEmail, string userPassword)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodeCode = WebUtility.UrlEncode(code);

            var body = $"{_webSettings.WebUrl}{_webSettings.ConfirmationEmailPath}?userid={user.Id}&code={encodeCode}";

            body = $"Potwierdź email klikając w <a href=\"{body}\">link</a> aby korzystać z konta, następnie zaloguj się." +
                   $"Login: {userEmail}" +
                   $"Hasło: {userPassword}";

            Mail mailRequest = new Mail()
            {
                ToEmail = user.Email,
                Subject = "Potwierdzenie adresu email",
                Body = body
            };

            return await _emailService.SendEmailAsync(mailRequest);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(IdentityUser user)
        {
            ApplicationUser appUser = await _userManager.FindByEmailAsync(user.Email);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("id", user.Id)
            };

            var userRoles = await _userManager.GetRolesAsync(appUser);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                UserId = appUser.Id
            };
        }

        private string GenerateRandomPassword()
        {
            PasswordOptions passwordOptions = new PasswordOptions()
            {
                RequiredLength = 10,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
            };

            CryptoRandom rand = new CryptoRandom();
            List<char> chars = new List<char>();

            if (passwordOptions.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (passwordOptions.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (passwordOptions.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (passwordOptions.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < passwordOptions.RequiredLength
                || chars.Distinct().Count() < passwordOptions.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        private async Task<bool> CheckIfUserExistAsync(int pesel)
        {
            return await _context.Users.AnyAsync(x => x.Pesel == pesel);
        } 
    }
}
