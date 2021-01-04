using System;
using System.Collections.Generic;
using System.Text;

namespace PublicElections.Domain.Models
{
    public class UserVote
    {
        public string UserId { get; set; }
        public int ElectionId { get; set; }
        public int CandidateId { get; set; }
    }
}
