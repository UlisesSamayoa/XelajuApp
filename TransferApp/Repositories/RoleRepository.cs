using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;
using TransferApp.Repositories.Interfaces;

namespace TransferApp.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _db;
        public RoleRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<RoleModel>> GetRoles()
        {
            var roles = new List<RoleModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetRoles", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                roles.Add(new RoleModel
                {
                    IdRole = Convert.ToInt32(reader["IdRole"]),
                    Name = reader["Name"].ToString()!,
                    Description = reader["Description"]?.ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    IsSystem = Convert.ToBoolean(reader["IsSystem"]),
                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                    CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"])
                });
            }
            return roles;
        }

        public async Task<List<RoleModel>> GetUserRoles(int idUser)
        {
            var roles = new List<RoleModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetUserRoles", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUser", idUser);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                roles.Add(new RoleModel
                {
                    IdRole = Convert.ToInt32(reader["IdRole"]),
                    Name = reader["Name"].ToString()!,
                    Description = reader["Description"]?.ToString()
                });
            }
            return roles;
        }
        public async Task<RoleModel?> GetRoleById(int idRole)
        {
            RoleModel? role = null;
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetRoleById", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdRole", idRole);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                role = new RoleModel
                {
                    IdRole = Convert.ToInt32(reader["IdRole"]),
                    Name = reader["Name"].ToString()!,
                    Description = reader["Description"] == DBNull.Value ? null : reader["Description"].ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    IsSystem = Convert.ToBoolean(reader["IsSystem"]),
                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                    CreatedBy = reader["CreatedBy"] == DBNull.Value ? null : Convert.ToInt32(reader["CreatedBy"])
                };
            }
            return role;
        }
        public async Task<SaveResultModel> SaveRole(RoleModel model)
        {
            throw new NotImplementedException();
        }
        public async Task<SaveResultModel> DeleteRole(int idRole)
        {
            throw new NotImplementedException();
        }



        public async Task<List<PermissionModel>> GetPermissions()
        {
            var permissions = new List<PermissionModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetPermissions", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(new PermissionModel
                {
                    IdPermission = Convert.ToInt32(reader["IdPermission"]),
                    Module = reader["Module"].ToString()!,
                    Action = reader["Action"].ToString()!,
                    Description = reader["Description"].ToString()!
                });
            }
            return permissions;
        }
        public async Task<List<PermissionModel>> GetRolePermissions(int idRole)
        {
            var permissions = new List<PermissionModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetRolePermissions", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdRole", idRole);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                permissions.Add(new PermissionModel
                {
                    IdPermission = Convert.ToInt32(reader["IdPermission"]),
                    Module = reader["Module"].ToString()!,
                    Action = reader["Action"].ToString()!,
                    Description = reader["Description"].ToString()!
                });
            }
            return permissions;
        }

        public async Task<SaveResultModel> SaveRolePermissions(int idRole, List<int> permissions)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_SaveRolePermissions", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdRole", idRole);
            //cmd.Parameters.AddWithValue("@Permissions", string.Join(",", permissions));
            cmd.Parameters.AddWithValue("@Permissions", permissions == null || permissions.Count == 0 ? "" : string.Join(",", permissions));
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new SaveResultModel
                {
                    Result = Convert.ToInt32(reader["Result"]),
                    Message = reader["Message"].ToString()!
                };
            }
            return new SaveResultModel
            {
                Result = -1,
                Message = "Unexpected error."
            };
        }


    }
}
