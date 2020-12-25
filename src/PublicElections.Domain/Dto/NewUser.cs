using System;

namespace PublicElections.Domain.Dto
{
    public class NewUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Pesel { get; set; }
        public string Email { get; set; }
    }
}
