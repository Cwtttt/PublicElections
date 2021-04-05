using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Contracts.Requests.Vote
{
    public class AddVoteRequest
    {
        [Required]
        public int CandidateId { get; set; }
        [Required]
        public int ElectionId { get; set; }
    }
}
