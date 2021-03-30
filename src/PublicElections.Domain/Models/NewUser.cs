using System;

namespace PublicElections.Domain.Models
{
    public class NewUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public string Email { get; set; }
    }
}
