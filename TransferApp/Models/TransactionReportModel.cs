public class TransactionReportModel
{
    public string ReferenceNumber { get; set; }
    public DateTime DateC { get; set; }
    public string ClientName { get; set; }
    public string CompanyName { get; set; }
    public decimal Amount { get; set; }
    public decimal Commission { get; set; }
    public decimal FixedCommission { get; set; }
    public decimal TotalCommission { get; set; }
    public decimal TotalAmount { get; set; }
    public byte CalculationMode { get; set; }
    public int TransactionType { get; set; }
}