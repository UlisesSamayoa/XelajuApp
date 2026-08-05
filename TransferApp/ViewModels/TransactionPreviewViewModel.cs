using TransferApp.Models;

namespace TransferApp.ViewModels
{
    public class TransactionPreviewViewModel
    {
        public int TransactionType { get; set; }
        public TransactionsModel? Transaction { get; set; }
        public SimpleTransactionsModel? SimpleTransaction { get; set; }
        public List<TransactionAttachmentModel> Attachments { get; set; } = new();
        public string StatusBadgeClass { get; set; } = "";
        public string StatusIcon { get; set; } = "";
        public string StatusText { get; set; } = "";
        public string? SenderPictureUrl { get; set; }
        public string? ServiceCompanyName { get; set; }
        public string? TransactionIcon { get; set; }
        public string TransactionTitle { get; set; }
        public string TransactionColor { get; set; }
    }
}
