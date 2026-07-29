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
        public async Task<List<RoleModel>> GetUserRoles(int idUser)
        {
            return await _repo.GetUserRoles(idUser);
        }
    }
}
