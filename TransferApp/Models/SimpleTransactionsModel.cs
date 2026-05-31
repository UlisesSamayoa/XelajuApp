namespace TransferApp.Models
{
    public class SimpleTransactionsModel
    {
        public int IdSimpleTransaction { get; set; }

        public int IdClient_fk { get; set; }

        public string ReferenceNumber { get; set; }

        public int TransactionType { get; set; }

        public int Company { get; set; }

        public decimal Amount { get; set; }

        public decimal Commission { get; set; }

        public decimal TotalAmount { get; set; }

        public string SenderName { get; set; }

        public int SenderDocumentType { get; set; }

        public string SenderDocumentNumber { get; set; }

        public string SenderPhone { get; set; }

        public string SenderAddress { get; set; }

        public string JustifyDetails { get; set; }

        public string? ImgJustify { get; set; }

        public decimal FixedCommission { get; set; }
        public int? Status { get; set; }

        public DateTime? DateC { get; set; }

        public string UserC { get; set; }

        public DateTime? DateU { get; set; }

        public string UserU { get; set; }
    }
}
