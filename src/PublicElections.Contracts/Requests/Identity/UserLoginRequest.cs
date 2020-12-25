using System.ComponentModel.DataAnnotations;

namespace PublicElections.Contracts.Requests.Identity
{
    public class UserLoginRequest
    {
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
