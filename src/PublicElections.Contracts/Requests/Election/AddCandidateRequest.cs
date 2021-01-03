using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicElections.Contracts.Requests.Election
{
    public class AddCandidateRequest
    {
        public string Name { get; set; }
        public int ElectionId { get; set; }
    }
}
