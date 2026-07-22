using TransferApp.Models;

namespace TransferApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserModel>> GetUsers();
        Task<UserModel?> GetUserById(int idUser);
        Task<SaveResultModel> SaveUser(UserModel model);
        Task<bool> DeleteUser(int idUser);
    }
}
