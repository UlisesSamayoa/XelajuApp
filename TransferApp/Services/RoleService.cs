using TransferApp.Models;
using TransferApp.Repositories.Interfaces;

namespace TransferApp.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _repo;
        public RoleService(IRoleRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<RoleModel>> GetRoles()
        {
            return await _repo.GetRoles();
        }
        public async Task<RoleModel?> GetRoleById(int idRole)
        {
            return await _repo.GetRoleById(idRole);
        }
        public async Task<List<RoleModel>> GetUserRoles(int idUser)
        {
            return await _repo.GetUserRoles(idUser);
        }

        public async Task<List<PermissionModel>> GetPermissions()
        {
            return await _repo.GetPermissions();
        }
        public async Task<List<PermissionModel>> GetRolePermissions(int idRole)
        {
            return await _repo.GetRolePermissions(idRole);
        }
        //public async Task<SaveResultModel> SaveRolePermissions(int idRole, List<int> permissions)
        //{
        //    return await _repo.SaveRolePermissions(idRole, permissions);
        //}
        public async Task<SaveResultModel> SaveRolePermissions(int idRole, List<int>? permissions)
        {
            permissions ??= new List<int>();
            return await _repo.SaveRolePermissions(idRole, permissions);
        }

    }
}
