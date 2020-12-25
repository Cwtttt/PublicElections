using System;
using System.ComponentModel.DataAnnotations;

namespace PublicElections.Contracts.Requests.Election
{
    public class CreateElectionRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
