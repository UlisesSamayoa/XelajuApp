using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class TransactionsRepository
{
    private readonly ApplicationDbContext _db;

    public TransactionsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<TransactionsModel>> GetAll()
    {
        var list = new List<TransactionsModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTransactions", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new TransactionsModel
            {
                IdTransaction = (int)rd["IdTransaction"],
                ReferenceNumber = rd["ReferenceNumber"].ToString(),
                SenderName = rd["SenderName"].ToString(),
                ReceiverName = rd["ReceiverName"].ToString(),
                ReceiverCountry = rd["ReceiverCountry"].ToString(),
                Amount = (decimal)rd["Amount"],
                Status = (int)rd["Status"]
            });
        }

        return list;
    }

    public async Task Create(TransactionsModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateTransaction", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@TransactionType", m.TransactionType);
        cmd.Parameters.AddWithValue("@Amount", m.Amount);
        cmd.Parameters.AddWithValue("@Currency", m.Currency);
        cmd.Parameters.AddWithValue("@ReferenceNumber", m.ReferenceNumber);

        cmd.Parameters.AddWithValue("@SenderCountry", m.SenderCountry);
        cmd.Parameters.AddWithValue("@SenderCompany", m.SenderCompany);
        cmd.Parameters.AddWithValue("@SenderCurrency", m.SenderCurrency);
        cmd.Parameters.AddWithValue("@SenderName", m.SenderName);
        cmd.Parameters.AddWithValue("@SenderDocumentType", m.SenderDocumentType);
        cmd.Parameters.AddWithValue("@SenderDocumentNumber", m.SenderDocumentNumber);
        cmd.Parameters.AddWithValue("@SenderPhone", m.SenderPhone ?? "");
        cmd.Parameters.AddWithValue("@SenderAddress", m.SenderAddress ?? "");

        cmd.Parameters.AddWithValue("@ReceiverCountry", m.ReceiverCountry);
        cmd.Parameters.AddWithValue("@ReceiverCompany", m.ReceiverCompany);
        cmd.Parameters.AddWithValue("@ReceiverCurrency", m.ReceiverCurrency);
        cmd.Parameters.AddWithValue("@ReceiverName", m.ReceiverName);
        cmd.Parameters.AddWithValue("@ReceiverDocumentType", m.ReceiverDocumentType);
        cmd.Parameters.AddWithValue("@ReceiverDocumentNumber", m.ReceiverDocumentNumber);
        cmd.Parameters.AddWithValue("@ReceiverPhone", m.ReceiverPhone ?? "");
        cmd.Parameters.AddWithValue("@ReceiverAddress", m.ReceiverAddress ?? "");

        cmd.Parameters.AddWithValue("@JustifyDetails", m.JustifyDetails ?? "");
        cmd.Parameters.AddWithValue("@UserC", m.UserC);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id, string user)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteTransaction", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdTransaction", id);
        cmd.Parameters.AddWithValue("@UserU", user);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}