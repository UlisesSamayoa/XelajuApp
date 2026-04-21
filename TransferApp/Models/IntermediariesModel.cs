using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TransferApp.Models
{
    public class IntermediariesModel
    {
        public int IdIntermediaries { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(50)]
        public string SwiftCode { get; set; }

        [StringLength(50)]
        public string RoutingNumber { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
