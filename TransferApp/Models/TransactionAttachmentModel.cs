namespace TransferApp.Models
{
    public class TransactionAttachmentModel
    {
        public long IdTransactionAttachment { get; set; }
        public int IdTransaction { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string AttachmentType { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
