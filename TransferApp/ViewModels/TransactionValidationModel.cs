using Microsoft.AspNetCore.Mvc;

namespace TransferApp.ViewModels
{
    public class TransactionValidationModel
    {
        public int TotalTransactions { get; set; }
        public decimal MaxAmount { get; set; }
        public int MaxTransactions { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
