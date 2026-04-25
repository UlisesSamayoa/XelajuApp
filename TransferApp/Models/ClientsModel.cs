using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TransferApp.Models
{
    public class ClientsModel
    {
        public int IdClient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdDocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string Address { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Picture { get; set; }
        public int Status { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; }
        public DateTime DateU { get; set; }
        public string UserU { get; set; }
        public string? CountryName { get; set; }
    }
}
