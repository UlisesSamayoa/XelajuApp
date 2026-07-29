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
            throw new NotImplementedException();
        }
        public async Task<SaveResultModel> SaveRole(RoleModel model)
        {
            throw new NotImplementedException();
        }
        public async Task<SaveResultModel> DeleteRole(int idRole)
        {
            throw new NotImplementedException();
        }





    }
}
