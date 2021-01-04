using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Contracts.Requests.Vote
{
    public class AddVoteRequest
    {
        public string UserId { get; set; }
        public int CandidateId { get; set; }
        public int ElectionId { get; set; }
    }
}
