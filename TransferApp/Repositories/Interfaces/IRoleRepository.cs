using TransferApp.Models;

namespace TransferApp.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<RoleModel>> GetRoles();
        Task<RoleModel?> GetRoleById(int idRole);
        Task<SaveResultModel> SaveRole(RoleModel model);
        Task<SaveResultModel> DeleteRole(int idRole);
        Task<List<RoleModel>> GetUserRoles(int idUser);

        Task<List<PermissionModel>> GetPermissions();
        Task<List<PermissionModel>> GetRolePermissions(int idRole);
        Task<SaveResultModel> SaveRolePermissions(int idRole, List<int> permissions);

    }
}
