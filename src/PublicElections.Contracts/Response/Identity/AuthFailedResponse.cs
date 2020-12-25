using System.Collections.Generic;

namespace PublicElections.Contracts.Response.Identity
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
