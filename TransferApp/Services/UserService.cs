using TransferApp.Models;
using TransferApp.Repositories.Interfaces;

namespace TransferApp.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }
        public Task<List<UserModel>> GetUsers()
            => _repo.GetUsers();

        public async Task<UserModel?> GetUserById(int idUser)
        {
            return await _repo.GetUserById(idUser);
        }

        public Task<SaveResultModel> SaveUser(UserModel model)
            => _repo.SaveUser(model);

        public Task<bool> DeleteUser(int id)
            => _repo.DeleteUser(id);
    }
}
