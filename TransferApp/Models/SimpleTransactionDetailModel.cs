namespace TransferApp.Models
{
    public class SimpleTransactionDetailModel
    {
        public string ReferenceNumber { get; set; }
        public int Company { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal FixedCommission { get; set; }
        public DateTime? IssueDateCheck { get; set; }
        public int CalculationMode { get; set; }
    }
}
