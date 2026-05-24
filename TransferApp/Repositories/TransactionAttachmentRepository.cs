using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

namespace TransferApp.Repositories
{
    public class TransactionAttachmentRepository : Controller
    {
        private readonly ApplicationDbContext _db;

        public TransactionAttachmentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TransactionAttachmentModel>> GetAttachments(int idTransaction)
        {
            var list = new List<TransactionAttachmentModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetTransactionAttachments", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdTransaction", idTransaction);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new TransactionAttachmentModel
                {
                    IdTransactionAttachment = Convert.ToInt64(reader["IdTransactionAttachment"]),
                    IdTransaction = Convert.ToInt32(reader["IdTransaction"]),
                    FileName = reader["FileName"]?.ToString() ?? string.Empty,
                    OriginalFileName = reader["OriginalFileName"]?.ToString() ?? string.Empty,
                    FileExtension = reader["FileExtension"]?.ToString() ?? string.Empty,
                    ContentType = reader["ContentType"]?.ToString() ?? string.Empty,
                    FilePath = reader["FilePath"]?.ToString() ?? string.Empty,
                    AttachmentType = reader["AttachmentType"]?.ToString() ?? string.Empty,
                    FileSize = reader["FileSize"] == DBNull.Value ? null : Convert.ToInt64(reader["FileSize"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    CreatedBy = reader["CreatedBy"]?.ToString() ?? string.Empty
                });
            }
            return list;
        }

        public async Task<long> CreateAttachment(TransactionAttachmentModel m)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_CreateTransactionAttachment", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdTransaction", m.IdTransaction);
            cmd.Parameters.AddWithValue("@FileName", m.FileName);
            cmd.Parameters.AddWithValue("@OriginalFileName", m.OriginalFileName);
            cmd.Parameters.AddWithValue("@FileExtension", (object?)m.FileExtension ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContentType", (object?)m.ContentType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FilePath", m.FilePath);
            cmd.Parameters.AddWithValue("@AttachmentType", m.AttachmentType);
            cmd.Parameters.AddWithValue("@FileSize", (object?)m.FileSize ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", (object?)m.CreatedBy ?? DBNull.Value);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }
    }
}
