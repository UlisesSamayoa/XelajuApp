namespace TransferApp.Models.Reports
{
    public class TransactionReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime GeneratedDate { get; set; }
        public int TransactionType { get; set; }
        public string TransactionTypeName { get; set; }
        public int CalculationMode { get; set; }
        public List<TransactionReportModel> Transactions { get; set; } = new();
        public string TypeName
        {
            get
            {
                return TransactionType switch
                {
                    1 => "CC",
                    2 => "MO",
                    3 => "MT",
                    4 => "PS",
                    5 => "DT",
                    _ => "-"
                };
            }
        }
        public string Settlement
        {
            get
            {
                return CalculationMode switch
                {
                    1 => "Customer Pays Fees",
                    2 => "Fees Deducted",
                    _ => "-"
                };
            }
        }
    }
}
