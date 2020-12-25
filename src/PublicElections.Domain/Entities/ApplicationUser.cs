using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace PublicElections.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public int Pesel { get; set; }
    }
}
