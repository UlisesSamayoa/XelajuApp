namespace TransferApp.Models
{
    public class PermissionModel
    {
        public int IdPermission { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
