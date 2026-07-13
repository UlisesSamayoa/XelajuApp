namespace TransferApp.Models.Reports
{
    public class NewClientReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public List<ClientItem> Clients { get; set; } = new();
        public class ClientItem
        {
            public string FullName { get; set; }
            public string DocumentNumber { get; set; }
            public string Address { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public DateTime? IssueDate { get; set; }
            public DateTime? Dob { get; set; }
            public string Phone { get; set; }
            public string Country { get; set; }
            public int Status { get; set; }
            public DateTime DateC { get; set; }
        }
    }
}
