using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PublicElections.Domain.Entities
{
    public class Participation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        [Key]
        [Column(Order = 1)]
        public int ElectionId { get; set; }
        public Election Election { get; set; }
    }
}
