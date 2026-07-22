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

        public Task<UserModel?> GetUserById(int id)
            => _repo.GetUserById(id);

        public Task<SaveResultModel> SaveUser(UserModel model)
            => _repo.SaveUser(model);

        public Task<bool> DeleteUser(int id)
            => _repo.DeleteUser(id);
    }
}
