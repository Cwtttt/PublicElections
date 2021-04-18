using System;
using System.Collections.Generic;
using System.Text;

namespace PublicElections.Domain.Models
{
    public class ElectionResult
    {
        public int VotesAmmount { get; set; }
        public List<CandidateVotesResult> CandidatesResults { get; set; } 
            = new List<CandidateVotesResult>();
    }

    public class CandidateVotesResult
    {
        public string CandidateName { get; set; }
        public decimal Percentages { get; set; }
    }
}
