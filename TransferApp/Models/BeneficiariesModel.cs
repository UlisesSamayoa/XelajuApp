using Microsoft.AspNetCore.Mvc;

namespace TransferApp.Models
{
    public class BeneficiariesModel
    {
        public int IdBeneficiarie { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdDocumentType { get; set; }
        public string DocumentTypeName { get; set; }
        public string DocumentNumber { get; set; }
        public string Country { get; set; }
        public int IdClient_fk { get; set; }
        public string ParentClientName { get; set; }
        public int Status { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; }
        public DateTime DateU { get; set; }
        public string UserU { get; set; }
        public string? CountryName { get; set; }

    }
}
