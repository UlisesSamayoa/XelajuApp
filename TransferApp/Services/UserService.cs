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
        private readonly RoleService _roleService;
        private readonly PermissionService _permissionService;
        public UserService(IUserRepository repo, PasswordService passwordService, RoleService roleService, PermissionService permissionService)
        {
            _repo = repo;
            _passwordService = passwordService;
            _roleService = roleService;
            _permissionService = permissionService;
        }
        public Task<List<UserModel>> GetUsers()
            => _repo.GetUsers();

        public async Task<UserModel?> GetUserById(int idUser)
        {
            return await _repo.GetUserById(idUser);
        }

        //public async Task<SaveResultModel> SaveUser(UserViewModel model)
        //{
        //    var user = new UserModel
        //    {
        //        IdUser = model.IdUser,
        //        Username = model.Username,
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Email = model.Email,
        //        IsActive = model.IsActive,
        //        MustChangePassword = model.MustChangePassword
        //    };
        //    if (model.IdUser == 0)
        //    {
        //        user.PasswordHash = _passwordService.HashPassword(model.Password!);
        //    }
        //    var result = await _repo.SaveUser(user);
        //    if (result.Result != 1 || result.Id == null)
        //        return result;
        //    var roleResult = await _repo.SaveUserRoles(result.Id.Value, model.SelectedRoles);
        //    if (roleResult.Result != 1)
        //        return roleResult;
        //    return result;
        //}
        public async Task<SaveResultModel> SaveUser(UserViewModel model)
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
            if (model.IdUser == 0)
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    return new SaveResultModel
                    {
                        Result = -1,
                        Message = "Password is required."
                    };
                }
                user.PasswordHash = _passwordService.HashPassword(model.Password);
            }
            var result = await _repo.SaveUser(user);
            if (result.Result != 1 || result.Id == null)
                return result;
            var roleResult = await _repo.SaveUserRoles(result.Id.Value, model.SelectedRoles);
            if (roleResult.Result != 1)
                return roleResult;
            return result;
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
            var result = await _repo.SaveUser(user);
            if (result.Result != 1)
                return result;

            return await _repo.SaveUserRoles(model.IdUser, model.SelectedRoles);
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
            user.Roles = await _roleService.GetUserRoles(user.IdUser);
            user.Permissions = await _permissionService.GetUserPermissions(user.IdUser);
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
