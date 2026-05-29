namespace TransferApp.Models
{
    public class SimpleTransactionsBatchModel
    {
        public int IdClient_fk { get; set; }
        public int TransactionType { get; set; }
        public string SenderName { get; set; }
        public int SenderDocumentType { get; set; }
        public string SenderDocumentNumber { get; set; }
        public string SenderPhone { get; set; }
        public string SenderAddress { get; set; }
        public string JustifyDetails { get; set; }
        public string UserC { get; set; }
        public List<SimpleTransactionDetailModel> Checks { get; set; }
    }
}
