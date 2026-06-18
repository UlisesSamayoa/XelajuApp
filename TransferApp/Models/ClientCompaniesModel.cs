namespace TransferApp.Models
{
    public class ClientCompaniesModel
    {
        public int IdClientCompany { get; set; }
        public int IdClient { get; set; }
        public int IdCompany { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string RoutingNumber { get; set; } = string.Empty;
        public int AccountType { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string AccountTypeName { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; } = string.Empty;
        public DateTime? DateU { get; set; }
        public string UserU { get; set; } = string.Empty;
    }
}
