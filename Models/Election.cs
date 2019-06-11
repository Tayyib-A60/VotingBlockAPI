using System;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Election
    {
        public int ElectionId { get; set; }
        [Required]
        [StringLength(255)]
        public string ElectionAddress { get; set; }
        public int UserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}