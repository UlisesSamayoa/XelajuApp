using TransferApp.Models;

public class ClientTransactionsReportViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime GeneratedDate { get; set; }
    public int TransactionType { get; set; }
    public string TransactionTypeName { get; set; }
    public int CalculationMode { get; set; }
    public ClientsModel Client { get; set; }
    public List<TransactionReportModel> Transactions { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalCommission { get; set; }
}