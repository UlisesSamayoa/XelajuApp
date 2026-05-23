namespace TransferApp.Models
{
    public class CompaniesModel
    {
        public int IdCompany { get; set; }
        public string Name { get; set; }
        public string SwiftCode { get; set; }
        public string Country { get; set; }
        public string IdCountry { get; set; }
        public string Phone { get; set; }
        public int TransactionType { get; set; }
        public string? TransactionTypeName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneContactPerson { get; set; }
        public string Position { get; set; }
        public string StatusCompany { get; set; }
        public int Status { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; }
        public DateTime DateU { get; set; }
        public string UserU { get; set; }
    }
}
