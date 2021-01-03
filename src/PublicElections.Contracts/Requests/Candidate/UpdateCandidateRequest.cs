using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Contracts.Requests.Candidate
{
    public class UpdateCandidateRequest
    {
        public string Name { get; set; }
        public int CandidateId { get; set; }
    }
}
