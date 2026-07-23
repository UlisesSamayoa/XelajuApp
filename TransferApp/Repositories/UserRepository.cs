using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;
using TransferApp.Repositories.Interfaces;
using TransferApp.ViewModels;

namespace TransferApp.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<UserModel>> GetUsers()
        {
            var list = new List<UserModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetUsers", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new UserModel
                {
                    IdUser = Convert.ToInt32(rd["IdUser"]),
                    Username = rd["Username"].ToString(),
                    FirstName = rd["FirstName"].ToString(),
                    LastName = rd["LastName"].ToString(),
                    Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString(),
                    IsActive = Convert.ToBoolean(rd["IsActive"]),
                    LastLogin = rd["LastLogin"] == DBNull.Value ? null : Convert.ToDateTime(rd["LastLogin"]),
                    MustChangePassword = Convert.ToBoolean(rd["MustChangePassword"])
                });
            }
            return list;
        }
        public async Task<UserModel?> GetUserById(int idUser)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetUserById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUser", idUser);
            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                return new UserModel
                {
                    IdUser = Convert.ToInt32(rd["IdUser"]),
                    Username = rd["Username"].ToString()!,
                    PasswordHash = rd["PasswordHash"].ToString()!,
                    FirstName = rd["FirstName"].ToString()!,
                    LastName = rd["LastName"].ToString()!,
                    Email = rd["Email"] == DBNull.Value ? null : rd["Email"].ToString(),
                    IsActive = Convert.ToBoolean(rd["IsActive"]),
                    FailedAttempts = Convert.ToInt32(rd["FailedAttempts"]),
                    LockedUntil = rd["LockedUntil"] == DBNull.Value ? null : Convert.ToDateTime(rd["LockedUntil"]),
                    LastLogin = rd["LastLogin"] == DBNull.Value ? null : Convert.ToDateTime(rd["LastLogin"]),
                    PasswordChangedDate = Convert.ToDateTime(rd["PasswordChangedDate"]),
                    CreatedDate = Convert.ToDateTime(rd["CreatedDate"]),
                    CreatedBy = rd["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(rd["CreatedBy"]),
                    MustChangePassword = Convert.ToBoolean(rd["MustChangePassword"])
                };
            }
            return null;
        }

        public async Task<SaveResultModel> SaveUser(UserModel model)
        {
            using var conn = _db.CreateConnection();

            using var cmd = new SqlCommand("sp_SaveUser", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            //cmd.Parameters.AddWithValue("@IdUser", model.IdUser);
            cmd.Parameters.AddWithValue("@IdUser", model.IdUser == 0 ? DBNull.Value : model.IdUser);
            cmd.Parameters.AddWithValue("@Username", model.Username);
            cmd.Parameters.AddWithValue("@PasswordHash", model.PasswordHash);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName);
            cmd.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(model.Email) ? DBNull.Value : model.Email);
            cmd.Parameters.AddWithValue("@IsActive", model.IsActive);
            cmd.Parameters.AddWithValue("@MustChangePassword", model.MustChangePassword);
            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                return new SaveResultModel
                {
                    Result = Convert.ToInt32(rd["Result"]),
                    Id = rd["IdUser"] == DBNull.Value ? null : Convert.ToInt32(rd["IdUser"]),
                    Message = rd["Message"].ToString() ?? ""
                };
            }
            return new SaveResultModel
            {
                Result = 0,
                Message = "No response from database."
            };
        }
        public async Task<bool> DeleteUser(int idUser)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel?> GetUserByUsername(string username)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetUserByUsername", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Username", username);
            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            if (await rd.ReadAsync())
            {
                return new UserModel
                {
                    IdUser = Convert.ToInt32(rd["IdUser"]),
                    Username = rd["Username"].ToString()!,
                    PasswordHash = rd["PasswordHash"].ToString()!,
                    FirstName = rd["FirstName"].ToString()!,
                    LastName = rd["LastName"].ToString()!,
                    Email = rd["Email"] as string,
                    IsActive = Convert.ToBoolean(rd["IsActive"]),
                    FailedAttempts = Convert.ToInt32(rd["FailedAttempts"]),
                    LockedUntil = rd["LockedUntil"] == DBNull.Value ? null : Convert.ToDateTime(rd["LockedUntil"]),
                    LastLogin = rd["LastLogin"] == DBNull.Value ? null : Convert.ToDateTime(rd["LastLogin"]),
                    PasswordChangedDate = Convert.ToDateTime(rd["PasswordChangedDate"]),
                    MustChangePassword = Convert.ToBoolean(rd["MustChangePassword"])
                };
            }
            return null;
        }

        public async Task<LoginResultModel> Authenticate(LoginViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
