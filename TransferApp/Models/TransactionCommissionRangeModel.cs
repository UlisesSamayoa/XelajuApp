namespace TransferApp.Models
{
    public class TransactionCommissionRangeModel
    {
        public int IdCommission { get; set; }
        public int IdTypeTransaction { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal CommissionPercent { get; set; }
        public int Status { get; set; }
        public int NumberT { get; set; }
        public DateTime? DateC { get; set; }
        public string UserC { get; set; }
        public DateTime? DateU { get; set; }
        public string UserU { get; set; }
    }
}
