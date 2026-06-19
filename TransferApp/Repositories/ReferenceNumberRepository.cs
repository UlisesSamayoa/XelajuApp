using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;

namespace TransferApp.Repositories
{
    public class ReferenceNumberRepository
    {
        private readonly ApplicationDbContext _db;

        public ReferenceNumberRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        //public async Task<string> GetNextReferenceNumber(int senderCompany, int transactionType)
        //{
        //    using var conn = _db.CreateConnection();
        //    using var cmd = new SqlCommand("sp_GetNextReferenceNumber", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@SenderCompany", senderCompany);
        //    cmd.Parameters.AddWithValue("@TransactionType", transactionType);
        //    await conn.OpenAsync();
        //    var result = await cmd.ExecuteScalarAsync();
        //    return result?.ToString() ?? "";
        //}
        public async Task<string> GetNextReferenceNumber(int senderCompany, int transactionType)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetNextReferenceNumber", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SenderCompany", senderCompany);
            cmd.Parameters.AddWithValue("@TransactionType", transactionType);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }
        public async Task<string> GetNextReferenceNumber_PS(int senderCompany, int transactionType, int ClientID)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetNextReferenceNumber", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SenderCompany", senderCompany);
            cmd.Parameters.AddWithValue("@TransactionType", transactionType);
            cmd.Parameters.AddWithValue("@IdClient", ClientID);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }
        public async Task<string> GetReferencePreview(int senderCompany, int transactionType)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetReferencePreview", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SenderCompany", senderCompany);
            cmd.Parameters.AddWithValue("@TransactionType", transactionType);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }
        public async Task<string> GetReferencePreview_PS(int senderCompany, int transactionType, int ClientID)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetReferencePreview", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SenderCompany", senderCompany);
            cmd.Parameters.AddWithValue("@TransactionType", transactionType);
            cmd.Parameters.AddWithValue("@IdClient", ClientID);
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return result?.ToString() ?? "";
        }
        public async Task SaveReferenceSequence(int senderCompany, int transactionType, long lastSequence)
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_SaveReferenceSequence", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SenderCompany", senderCompany);
            cmd.Parameters.AddWithValue("@TransactionType", transactionType);
            cmd.Parameters.AddWithValue("@LastSequence", lastSequence);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

    }
}
