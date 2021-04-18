using System;

namespace PublicElections.Domain.Models
{
    public class NewUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Pesel { get; set; }
        public string IdNumber { get; set; }
        public string Street { get; set; }
        public string HauseNumber { get; set; }
        public string ApartmentNumber { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
    }
}
