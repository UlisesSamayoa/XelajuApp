using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;
using TransferApp.Repositories.Interfaces;

namespace TransferApp.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ApplicationDbContext _db;
        public PermissionRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<List<PermissionModel>> GetUserPermissions(int idUser)
        {
            var permissions = new List<PermissionModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetUserPermissions", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdUser", idUser);
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


    }
}
