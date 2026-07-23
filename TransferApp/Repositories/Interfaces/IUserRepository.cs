using TransferApp.Models;
using TransferApp.ViewModels;

namespace TransferApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserModel>> GetUsers();
        Task<UserModel?> GetUserById(int idUser);
        Task<SaveResultModel> SaveUser(UserModel model);
        Task<bool> DeleteUser(int idUser);
        Task<UserModel?> GetUserByUsername(string username);
        Task<LoginResultModel> Authenticate(LoginViewModel model);
    }
}
