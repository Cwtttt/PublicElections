using System;

namespace PublicElections.Contracts.Requests.Election
{
    public class UpdateElectionRequest
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
