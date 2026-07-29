namespace TransferApp.Models
{
    public class RoleModel
    {
        public int IdRole { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }
}
