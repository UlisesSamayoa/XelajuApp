using TransferApp.Models;
using TransferApp.Repositories.Interfaces;
using TransferApp.Security;
using TransferApp.ViewModels;

namespace TransferApp.Services
{
    public class UserService
    {
        private readonly IUserRepository _repo;
        private readonly PasswordService _passwordService;

        public UserService(IUserRepository repo, PasswordService passwordService)
        {
            _repo = repo;
            _passwordService = passwordService;
        }
        public Task<List<UserModel>> GetUsers()
            => _repo.GetUsers();

        public async Task<UserModel?> GetUserById(int idUser)
        {
            return await _repo.GetUserById(idUser);
        }

        //public async Task<SaveResultModel> SaveUser(UserViewModel model)
        //{
        //    if (model.Password != model.ConfirmPassword)
        //    {
        //        return new SaveResultModel
        //        {
        //            Result = -2,
        //            Message = "Passwords do not match."
        //        };
        //    }
        //    var user = new UserModel
        //    {
        //        IdUser = model.IdUser,
        //        Username = model.Username,
        //        PasswordHash = _passwordService.HashPassword(model.Password),
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Email = model.Email,
        //        IsActive = model.IsActive,
        //        MustChangePassword = model.MustChangePassword
        //    };

        //    return await _repo.SaveUser(user);
        //}
        public async Task<SaveResultModel> SaveUser(UserViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return new SaveResultModel
                {
                    Result = -2,
                    Message = "Passwords do not match."
                };
            }
            var user = new UserModel
            {
                Username = model.Username,
                PasswordHash = _passwordService.HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                IsActive = model.IsActive,
                MustChangePassword = model.MustChangePassword
            };
            return await _repo.SaveUser(user);
        }

        public async Task<SaveResultModel> SaveUser(UserEditViewModel model)
        {
            var user = new UserModel
            {
                IdUser = model.IdUser,
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                IsActive = model.IsActive,
                MustChangePassword = model.MustChangePassword
            };
            return await _repo.SaveUser(user);
        }

        public Task<bool> DeleteUser(int id)
            => _repo.DeleteUser(id);

        public async Task<UserModel?> GetUserByUsername(string username)
        {
            return await _repo.GetUserByUsername(username);
        }
        public async Task<LoginResultModel> Authenticate(LoginViewModel model)
        {
            var user = await _repo.GetUserByUsername(model.Username);
            if (user == null)
            {
                return new LoginResultModel
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }
            bool valid = _passwordService.VerifyPassword(user.PasswordHash, model.Password);
            if (!valid)
            {
                return new LoginResultModel
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }
            return new LoginResultModel
            {
                Success = true,
                User = user
            };
        }
    }
}
