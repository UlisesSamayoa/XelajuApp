using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TransferApp.Models
{
    public class CountriesModel
    {
        public int IdCountry { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        [StringLength(10)]
        public string Currency { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
