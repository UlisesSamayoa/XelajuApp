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






        //REFACTORIZACION
        public async Task<LoginResultModel> Authenticate(LoginViewModel model)
        {
            var user = await GetUserByUsername(model.Username);
            if (user == null)
            {
                return new LoginResultModel
                {
                    Success = false,
                    Message = "Invalid username or password."
                };
            }
            var statusResult = ValidateAccountStatus(user);
            if (statusResult != null)
                return statusResult;
            bool valid = _passwordService.VerifyPassword(user.PasswordHash, model.Password);
            if (!valid)
                return await HandleFailedLogin(user);
            return await HandleSuccessfulLogin(user);
        }

        private LoginResultModel? ValidateAccountStatus(UserModel user)
        {
            if (!user.IsActive)
            {
                return new LoginResultModel
                {
                    Success = false,
                    Message = "This account is inactive. Contact the administrator."
                };
            }
            if (user.LockedUntil.HasValue &&
                user.LockedUntil.Value > DateTime.Now)
            {
                return new LoginResultModel
                {
                    Success = false,
                    IsLocked = true,
                    LockedUntil = user.LockedUntil,
                    Message = $"Account locked until {user.LockedUntil:hh:mm tt}."
                };
            }
            return null;
        }

        private async Task<LoginResultModel> HandleFailedLogin(UserModel user)
        {
            var failed = await ProcessFailedLogin(user.IdUser);
            bool locked = failed.LockedUntil.HasValue && failed.LockedUntil > DateTime.Now;
            return new LoginResultModel
            {
                Success = false,
                FailedAttempts = failed.FailedAttempts,
                IsLocked = locked,
                LockedUntil = failed.LockedUntil,
                Message = locked
                    ? $"Your account has been locked until {failed.LockedUntil:hh:mm tt}."
                    : "Invalid username or password."
            };
        }

        private async Task<LoginResultModel> HandleSuccessfulLogin(UserModel user)
        {
            await ResetLoginAttempts(user.IdUser);
            return new LoginResultModel
            {
                Success = true,
                MustChangePassword = user.MustChangePassword,
                User = user
            };
        }




        //REFACTORIZACION
        public async Task<SaveResultModel> ChangePassword(int idUser, string newPassword)
        {
            string hash = _passwordService.HashPassword(newPassword);
            return await _repo.ChangePassword(idUser, hash);
        }





        public async Task<FailedLoginResultModel> ProcessFailedLogin(int idUser)
        {
            return await _repo.ProcessFailedLogin(idUser);
        }
        public async Task ResetLoginAttempts(int idUser)
        {
            await _repo.ResetLoginAttempts(idUser);
        }

    }
}
