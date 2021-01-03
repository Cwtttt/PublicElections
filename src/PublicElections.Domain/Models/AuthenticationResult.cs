using System.Collections.Generic;

namespace PublicElections.Domain.Models
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
