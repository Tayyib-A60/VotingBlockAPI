using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; }
        [Required]
        public Role UserRole { get; set; }
        public int NoOfUpdates { get; set; }
        public DateTime LastPasswordChange { get; set; }
        public ICollection<Election> Elections { get; set; }
    }
}