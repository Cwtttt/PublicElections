using System;
using System.Collections.Generic;
using System.Text;

namespace PublicElections.Domain.Models
{
    public class ElectionForUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool CanParticipate { get; set; }
    }
}
