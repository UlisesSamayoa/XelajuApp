using TransferApp.Models;

namespace TransferApp.ViewModels
{
    public class RoleViewModel
    {
        public int IdRole { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PermissionModel> AvailablePermissions { get; set; }
        public List<int> SelectedPermissions { get; set; }
    }
}
