namespace TransferApp.Models
{
    public class TransactionsModel
    {
        public int IdTransaction { get; set; }
        public int IdClient_fk { get; set; }
        public int IdBeneficiarie_fk { get; set; }
        public int TransactionType { get; set; }
        public string? TransactionTypeName { get; set; }
        public decimal Amount { get; set; }
        public decimal Commission { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;
        //SENDER
        public int SenderClientId { get; set; }
        public string SenderCountry { get; set; } = string.Empty;
        public string? SenderCountryName { get; set; }
        public int SenderCompany { get; set; }
        public string SenderCompanyName { get; set; } = string.Empty;
        public int SenderCurrency { get; set; }
        public string SenderCurrencyName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public int SenderDocumentType { get; set; }
        public string SenderDocumentNumber { get; set; } = string.Empty;
        public string SenderPhone { get; set; } = string.Empty;
        public string SenderAddress { get; set; } = string.Empty;

        //RECEIVER
        public int ReceiverClientId { get; set; }
        public string ReceiverCountry { get; set; } = string.Empty;
        public string? ReceiverCountryName { get; set; }
        public int ReceiverCompany { get; set; }
        public string ReceiverCompanyName { get; set; } = string.Empty;
        public int ReceiverCurrency { get; set; }
        public string ReceiverCurrencyName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public int ReceiverDocumentType { get; set; }
        public string ReceiverDocumentNumber { get; set; } = string.Empty;
        public string? ReceiverPhone { get; set; }
        public string? ReceiverAddress { get; set; }

        public string JustifyDetails { get; set; } = string.Empty;
        public string TransactionFile { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public string TransactionsStatusComment { get; set; } = string.Empty;
        public string Justify_AgentName { get; set; } = string.Empty;
        public DateTime? Justify_DateError { get; set; }

        //OTROS
        public decimal FixedCommission { get; set; }
        public DateTime? IssueDateCheck { get; set; }
        public bool IsSimpleTransaction { get; set; }
        public string? Notes { get; set; }
        public int CalculationMode { get; set; }
        public string CalculationModeName { get; set; }
        public string ServiceCompanyPS { get; set; }
        public int? Status { get; set; }
        public DateTime? DateC { get; set; }
        public string? UserC { get; set; }
        public DateTime? DateU { get; set; }
        public string? UserU { get; set; }
    }
}
