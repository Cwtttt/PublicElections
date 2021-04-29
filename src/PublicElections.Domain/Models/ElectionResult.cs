using System;
using System.Collections.Generic;
using System.Text;

namespace PublicElections.Domain.Models
{
    public class ElectionResult
    {
        public int VotesAmount { get; set; }
        public int VotesAuthorized { get; set; } = 180;
        public decimal Attendance 
        {
            get
            {
                if(VotesAmount > 0)
                {
                    return Math.Round(100.00m * VotesAmount / (decimal)VotesAuthorized, 3);
                }
                else
                {
                    return 0.0m;
                }
            }
        }
        public List<CandidateVotesResult> CandidatesResults { get; set; } 
            = new List<CandidateVotesResult>();
    }

    public class CandidateVotesResult
    {
        public string CandidateName { get; set; }
        public decimal Percentages { get; set; }
        public int VotesCount { get; set; }
    }
}
