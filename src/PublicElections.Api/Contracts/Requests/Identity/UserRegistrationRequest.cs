using System;
using System.ComponentModel.DataAnnotations;

namespace PublicElections.Api.Contracts.Requests.Identity
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
        public string Password { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "The field Accept terms must be checked.")]
        public bool UserTermsAgreement { get; set; }
    }
}
