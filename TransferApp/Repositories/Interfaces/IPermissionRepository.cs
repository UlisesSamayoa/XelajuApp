using TransferApp.Models;

namespace TransferApp.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<List<PermissionModel>> GetUserPermissions(int idUser);
    }
}
