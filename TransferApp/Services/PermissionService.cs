using TransferApp.Models;
using TransferApp.Repositories.Interfaces;

namespace TransferApp.Services
{
    public class PermissionService
    {
        private readonly IPermissionRepository _repo;
        public PermissionService(IPermissionRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<PermissionModel>> GetUserPermissions(int idUser)
        {
            return await _repo.GetUserPermissions(idUser);
        }
    }
}
