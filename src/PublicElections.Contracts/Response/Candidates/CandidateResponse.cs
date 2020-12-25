using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Contracts.Response.Candidates
{
    public class CandidateResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ElectionId { get; set; }
    }
}
