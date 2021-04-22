using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PublicElections.Domain.Entities
{
    public class Candidate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int ElectionId { get; set; }
        public Election Election { get; set; }
    }
}
