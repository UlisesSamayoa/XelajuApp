namespace TransferApp.Models
{
    public class DocumentsTypes
    {
        public int IdDocumentType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string MaskPattern { get; set; }
        public DateTime DateC { get; set; }
        public string UserC { get; set; }
        public DateTime DateU { get; set; }
        public string UserU { get; set; }
    }
}
