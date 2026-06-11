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
                IdTransaction = rd["IdTransaction"] != DBNull.Value ? Convert.ToInt32(rd["IdTransaction"]) : 0,
                TransactionType = rd["TransactionType"] != DBNull.Value ? Convert.ToInt32(rd["TransactionType"]) : 0,
                ReferenceNumber = rd["ReferenceNumber"] != DBNull.Value ? rd["ReferenceNumber"].ToString() : string.Empty,
                SenderName = rd["SenderName"] != DBNull.Value ? rd["SenderName"].ToString() : string.Empty,
                SenderCompany = rd["SenderCompany"] != DBNull.Value ? Convert.ToInt32(rd["SenderCompany"]) : 0,
                SenderCompanyName = rd["SenderCompanyName"]?.ToString(),
                SenderDocumentNumber = rd["SenderDocumentNumber"] != DBNull.Value ? rd["SenderDocumentNumber"].ToString() : string.Empty,
                ReceiverName = rd["ReceiverName"] != DBNull.Value ? rd["ReceiverName"].ToString() : string.Empty,
                ReceiverDocumentNumber = rd["ReceiverDocumentNumber"] != DBNull.Value ? rd["ReceiverDocumentNumber"].ToString() : string.Empty,
                ReceiverCountry = rd["ReceiverCountry"] != DBNull.Value ? rd["ReceiverCountry"].ToString() : string.Empty,
                ReceiverCountryName = rd["ReceiverCountryName"] != DBNull.Value ? rd["ReceiverCountryName"].ToString() : string.Empty,
                ReceiverCompany = rd["ReceiverCompany"] != DBNull.Value ? Convert.ToInt32(rd["ReceiverCompany"]) : 0,
                ReceiverCompanyName = rd["ReceiverCompanyName"]?.ToString(),
                TransactionTypeName = rd["TransactionTypeName"] != DBNull.Value ? rd["TransactionTypeName"].ToString() : string.Empty,
                TransactionFile = rd["TransactionFile"] != DBNull.Value ? rd["TransactionFile"].ToString() : string.Empty,
                TransactionStatus = rd["TransactionStatus"] != DBNull.Value ? rd["TransactionStatus"].ToString() : string.Empty,
                Amount = rd["Amount"] != DBNull.Value ? Convert.ToDecimal(rd["Amount"]) : 0,
                Status = rd["Status"] != DBNull.Value ? Convert.ToInt32(rd["Status"]) : 0,
                CalculationMode = rd["CalculationMode"] != DBNull.Value ? Convert.ToInt32(rd["CalculationMode"]) : 1
            });
        }
        await conn.CloseAsync();
        return list;
    }
    public async Task<int> Create(TransactionsModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateTransaction", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@TransactionType", m.TransactionType);
        cmd.Parameters.AddWithValue("@Amount", m.Amount);
        cmd.Parameters.AddWithValue("@Commission", m.Commission);
        cmd.Parameters.AddWithValue("@FixedCommission", m.FixedCommission);
        cmd.Parameters.AddWithValue("@TotalAmount", m.TotalAmount);
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
        cmd.Parameters.AddWithValue("@IdClient_fk", m.IdClient_fk);
        cmd.Parameters.AddWithValue("@IdBeneficiarie_fk", m.IdBeneficiarie_fk);
        cmd.Parameters.AddWithValue("@JustifyDetails", m.JustifyDetails ?? "");
        cmd.Parameters.AddWithValue("@TransactionFile", m.TransactionFile ?? "");
        cmd.Parameters.AddWithValue("@Justify_AgentName", m.Justify_AgentName ?? "");
        cmd.Parameters.AddWithValue("@Justify_DateError", m.Justify_DateError == null ? (object)DBNull.Value : m.Justify_DateError);
        cmd.Parameters.AddWithValue("@CalculationMode", m.CalculationMode);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        await conn.CloseAsync();
        return Convert.ToInt32(result);
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
        await conn.CloseAsync();
    }

    public async Task<TransactionsModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTransactionById", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdTransaction", id);
        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();
        if (await rd.ReadAsync())
        {
            return new TransactionsModel
            {
                IdTransaction = rd["IdTransaction"] != DBNull.Value ? Convert.ToInt32(rd["IdTransaction"]) : 0,
                ReferenceNumber = rd["ReferenceNumber"]?.ToString(),
                SenderName = rd["SenderName"]?.ToString(),
                SenderDocumentNumber = rd["SenderDocumentNumber"]?.ToString(),
                SenderPhone = rd["SenderPhone"]?.ToString(),
                SenderAddress = rd["SenderAddress"]?.ToString(),
                SenderCompany = rd["SenderCompany"] != DBNull.Value ? Convert.ToInt32(rd["SenderCompany"]) : 0,
                SenderCompanyName = rd["SenderCompanyName"]?.ToString(),
                ReceiverName = rd["ReceiverName"]?.ToString(),
                ReceiverDocumentNumber = rd["ReceiverDocumentNumber"]?.ToString(),
                ReceiverPhone = rd["ReceiverPhone"]?.ToString(),
                ReceiverAddress = rd["ReceiverAddress"]?.ToString(),
                ReceiverCompany = rd["ReceiverCompany"] != DBNull.Value ? Convert.ToInt32(rd["ReceiverCompany"]) : 0,
                ReceiverCompanyName = rd["ReceiverCompanyName"]?.ToString(),
                ReceiverCountryName = rd["ReceiverCountryName"]?.ToString(),
                Amount = rd["Amount"] != DBNull.Value ? Convert.ToDecimal(rd["Amount"]) : 0,
                Commission = rd["Commission"] != DBNull.Value ? Convert.ToDecimal(rd["Commission"]) : 0,
                TotalAmount = rd["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(rd["TotalAmount"]) : 0,
                JustifyDetails = rd["JustifyDetails"]?.ToString(),
                TransactionFile = rd["TransactionFile"]?.ToString(),
                Status = rd["Status"] != DBNull.Value ? Convert.ToInt32(rd["Status"]) : 0,
                TransactionTypeName = rd["TransactionTypeName"] != DBNull.Value ? rd["TransactionTypeName"].ToString() : string.Empty,
                SenderCountryName = rd["SenderCountryName"] != DBNull.Value ? rd["SenderCountryName"].ToString() : string.Empty,
                TransactionStatus = rd["TransactionStatus"] != DBNull.Value ? rd["TransactionStatus"].ToString() : string.Empty,
                CalculationMode = rd["CalculationMode"] != DBNull.Value ? Convert.ToInt32(rd["CalculationMode"]) : 1
            };
        }
        await conn.CloseAsync();
        return null;
    }
    public async Task<int> CreateSimple(SimpleTransactionsModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateSimpleTransaction", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ReferenceNumber", m.ReferenceNumber);
        cmd.Parameters.AddWithValue("@TransactionType", m.TransactionType);
        cmd.Parameters.AddWithValue("@Company", m.Company);
        cmd.Parameters.AddWithValue("@Amount", m.Amount);
        cmd.Parameters.AddWithValue("@Commission", m.Commission);
        cmd.Parameters.AddWithValue("@FixedCommission", m.FixedCommission);
        //cmd.Parameters.AddWithValue("@IssueDateCheck", m.IssueDateCheck);
        cmd.Parameters.Add("@IssueDateCheck", SqlDbType.Date).Value = (object?)m.IssueDateCheck ?? DBNull.Value;
        cmd.Parameters.AddWithValue("@TotalAmount", m.TotalAmount);
        cmd.Parameters.AddWithValue("@SenderName", m.SenderName);
        cmd.Parameters.AddWithValue("@SenderDocumentType", m.SenderDocumentType);
        cmd.Parameters.AddWithValue("@SenderDocumentNumber", m.SenderDocumentNumber);
        cmd.Parameters.AddWithValue("@JustifyDetails", m.JustifyDetails ?? "");
        cmd.Parameters.AddWithValue("@Justify_AgentName", m.Justify_AgentName ?? "");
        cmd.Parameters.AddWithValue("@Justify_DateError", m.Justify_DateError == null ? (object)DBNull.Value : m.Justify_DateError);
        cmd.Parameters.AddWithValue("@SenderPhone", m.SenderPhone);
        cmd.Parameters.AddWithValue("@SenderAddress", m.SenderAddress);
        cmd.Parameters.AddWithValue("@CalculationMode", m.CalculationMode);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        cmd.Parameters.AddWithValue("@IdClient_fk", m.IdClient_fk);
        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        await conn.CloseAsync();
        return Convert.ToInt32(result);
    }
    public async Task<int> CreateMorder(SimpleTransactionsModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateSimpleTransaction", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ReferenceNumber", m.ReferenceNumber);
        cmd.Parameters.AddWithValue("@TransactionType", m.TransactionType);
        cmd.Parameters.AddWithValue("@Company", m.Company);
        cmd.Parameters.AddWithValue("@Amount", m.Amount);
        cmd.Parameters.AddWithValue("@Commission", m.Commission);
        cmd.Parameters.AddWithValue("@FixedCommission", m.FixedCommission);
        //cmd.Parameters.AddWithValue("@IssueDateCheck", m.IssueDateCheck);
        cmd.Parameters.Add("@IssueDateCheck", SqlDbType.Date).Value = (object?)m.IssueDateCheck ?? DBNull.Value;
        cmd.Parameters.AddWithValue("@TotalAmount", m.TotalAmount);
        cmd.Parameters.AddWithValue("@SenderName", m.SenderName);
        cmd.Parameters.AddWithValue("@SenderDocumentType", m.SenderDocumentType);
        cmd.Parameters.AddWithValue("@SenderDocumentNumber", m.SenderDocumentNumber);
        cmd.Parameters.AddWithValue("@JustifyDetails", m.JustifyDetails ?? "");
        cmd.Parameters.AddWithValue("@Justify_AgentName", m.Justify_AgentName ?? "");
        cmd.Parameters.AddWithValue("@Justify_DateError", m.Justify_DateError == null ? (object)DBNull.Value : m.Justify_DateError);
        cmd.Parameters.AddWithValue("@SenderPhone", m.SenderPhone);
        cmd.Parameters.AddWithValue("@SenderAddress", m.SenderAddress);
        cmd.Parameters.AddWithValue("@CalculationMode", m.CalculationMode);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        cmd.Parameters.AddWithValue("@IdClient_fk", m.IdClient_fk);
        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        await conn.CloseAsync();
        return Convert.ToInt32(result);
    }
    public async Task<int> CreatePService(SimpleTransactionsModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateSimpleTransaction", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ReferenceNumber", m.ReferenceNumber);
        cmd.Parameters.AddWithValue("@TransactionType", m.TransactionType);
        cmd.Parameters.AddWithValue("@Company", m.Company);
        cmd.Parameters.AddWithValue("@Amount", m.Amount);
        cmd.Parameters.AddWithValue("@Commission", m.Commission);
        cmd.Parameters.AddWithValue("@FixedCommission", m.FixedCommission);
        //cmd.Parameters.AddWithValue("@IssueDateCheck", m.IssueDateCheck);
        cmd.Parameters.Add("@IssueDateCheck", SqlDbType.Date).Value = (object?)m.IssueDateCheck ?? DBNull.Value;
        cmd.Parameters.AddWithValue("@TotalAmount", m.TotalAmount);
        cmd.Parameters.AddWithValue("@SenderName", m.SenderName);
        cmd.Parameters.AddWithValue("@SenderDocumentType", m.SenderDocumentType);
        cmd.Parameters.AddWithValue("@SenderDocumentNumber", m.SenderDocumentNumber);
        cmd.Parameters.AddWithValue("@JustifyDetails", m.JustifyDetails ?? "");
        cmd.Parameters.AddWithValue("@Justify_AgentName", m.Justify_AgentName ?? "");
        cmd.Parameters.AddWithValue("@Justify_DateError", m.Justify_DateError == null ? (object)DBNull.Value : m.Justify_DateError);
        cmd.Parameters.AddWithValue("@SenderPhone", m.SenderPhone);
        cmd.Parameters.AddWithValue("@SenderAddress", m.SenderAddress);
        cmd.Parameters.AddWithValue("@CalculationMode", m.CalculationMode);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        cmd.Parameters.AddWithValue("@IdClient_fk", m.IdClient_fk);
        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        await conn.CloseAsync();
        return Convert.ToInt32(result);
    }
    public async Task ChangeStatus(int idTransaction, string status, string TransactionsStatusComment)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_Transactions_ChangeStatus", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdTransaction", idTransaction);
        cmd.Parameters.AddWithValue("@Status", status);
        cmd.Parameters.AddWithValue("@TransactionsStatusComment", TransactionsStatusComment ?? "");
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }


}