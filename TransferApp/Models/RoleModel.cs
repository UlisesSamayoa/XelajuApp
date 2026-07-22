namespace TransferApp.Models
{
    public class RoleModel
    {
        public int IdRole { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
