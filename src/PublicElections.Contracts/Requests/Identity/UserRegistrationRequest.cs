﻿using System.ComponentModel.DataAnnotations;

namespace PublicElections.Contracts.Requests.Identity
{
    public class UserRegistrationRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Pesel { get; set; }
        
        
    }
}
