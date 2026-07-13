namespace TransferApp.Models.Reports
{
    public class TransactionReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public int TransactionType { get; set; }
        public string TransactionTypeName { get; set; }
        public List<TransactionReportModel> Transactions { get; set; } = new();
    }
}
