using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TransferApp.Models
{
    public class UsersModel
    {
        public int IdUser { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        [Phone]
        public string Phone { get; set; }
        public string Address { get; set; }
        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
