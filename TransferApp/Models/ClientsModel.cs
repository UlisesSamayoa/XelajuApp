using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TransferApp.Models
{
    public class ClientsModel
    {
        public int IdClient { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        public string DocumentNumber { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
