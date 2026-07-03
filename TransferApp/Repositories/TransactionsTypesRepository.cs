using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class TransactionsTypesRepository
{
    private readonly ApplicationDbContext _db;

    public TransactionsTypesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<TransactionsTypesModel>> GetAll()
    {
        var list = new List<TransactionsTypesModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTypesTransactions", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new TransactionsTypesModel
            {
                IdTypeTransaction = (int)rd["IdTypeTransaction"],
                Name = rd["Name"].ToString(),
                //Commission = (decimal)rd["Commission"],
                Description = rd["Description"].ToString(),
                NumberT = (int)rd["NumberT"],
                Status = (int)rd["Status"]
            });
        }
        await conn.CloseAsync();
        return list;
    }

    public async Task<TransactionsTypesModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTypeTransactionById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdTypeTransaction", id);

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        if (await rd.ReadAsync())
        {
            return new TransactionsTypesModel
            {
                IdTypeTransaction = (int)rd["IdTypeTransaction"],
                Name = rd["Name"].ToString(),
                Commission = (decimal)rd["Commission"],
                Description = rd["Description"].ToString(),
                NumberT = (int)rd["NumberT"],
                Status = (int)rd["Status"]
            };
        }
        await conn.CloseAsync();
        return null;
    }
    public async Task<List<TransactionsTypesModel>> GetByNumber(int id)
    {
        var list = new List<TransactionsTypesModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTypeTransactionByNumber", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdTypeTransaction", id);

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new TransactionsTypesModel
            {
                IdTypeTransaction = (int)rd["IdTypeTransaction"],
                Name = rd["Name"].ToString(),
                Commission = (decimal)rd["Commission"],
                Description = rd["Description"].ToString(),
                NumberT = (int)rd["NumberT"],
                Status = (int)rd["Status"]
            });
        }
        await conn.CloseAsync();
        return list;
    }
    public async Task<List<TransactionCommissionRangeModel>> GetCommissionRanges(int numberT)
    {
        var list = new List<TransactionCommissionRangeModel>();
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetCommissionRangesByNumberT", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@NumberT", numberT);
        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            list.Add(
                new TransactionCommissionRangeModel
                {
                    IdCommission = Convert.ToInt32(rd["IdCommission"]),
                    NumberT = Convert.ToInt32(rd["NumberT"]),
                    MinAmount = Convert.ToDecimal(rd["MinAmount"]),
                    MaxAmount = Convert.ToDecimal(rd["MaxAmount"]),
                    CommissionPercent = Convert.ToDecimal(rd["CommissionPercent"]),
                    CommissionType = Convert.ToInt32(rd["CommissionType"]),
                    Status = Convert.ToInt32(rd["Status"])
                });
        }
        return list;
    }

    //public async Task Create(TransactionsTypesModel m)
    //{
    //    using var conn = _db.CreateConnection();
    //    using var cmd = new SqlCommand("sp_CreateTypeTransaction", conn);

    //    cmd.CommandType = CommandType.StoredProcedure;

    //    cmd.Parameters.AddWithValue("@Name", m.Name);
    //    cmd.Parameters.AddWithValue("@Commission", m.Commission);
    //    cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
    //    cmd.Parameters.AddWithValue("@NumberT", m.NumberT);
    //    cmd.Parameters.AddWithValue("@UserC", m.UserC);

    //    await conn.OpenAsync();
    //    await cmd.ExecuteNonQueryAsync();
    //    await conn.CloseAsync();
    //}
    public async Task<int> Create(TransactionsTypesModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateTypeTransaction", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Name", m.Name);
        cmd.Parameters.AddWithValue("@Commission", m.Commission ?? 0);
        cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
        cmd.Parameters.AddWithValue("@NumberT", m.NumberT);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }
    public async Task CreateCommissionRange(TransactionCommissionRangeModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateTypeTransactionCommission", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@NumberT", m.NumberT);
        cmd.Parameters.AddWithValue("@MinAmount", m.MinAmount);
        cmd.Parameters.AddWithValue("@MaxAmount", m.MaxAmount);
        cmd.Parameters.AddWithValue("@CommissionPercent", m.CommissionPercent);
        cmd.Parameters.AddWithValue("@CommissionType", m.CommissionType);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
    //public async Task<List<TransactionCommissionRangeModel>> GetCommissionRanges(int numberT)
    //{
    //    List<TransactionCommissionRangeModel> list = new();
    //    using var conn = _db.CreateConnection();
    //    using var cmd = new SqlCommand("sp_GetTypeTransactionCommissions", conn);
    //    cmd.CommandType = CommandType.StoredProcedure;
    //    cmd.Parameters.AddWithValue("@NumberT", numberT);
    //    await conn.OpenAsync();
    //    using var rd = await cmd.ExecuteReaderAsync();

    //    while (await rd.ReadAsync())
    //    {
    //        list.Add(
    //            new TransactionCommissionRangeModel
    //            {
    //                NumberT = Convert.ToInt32(rd["NumberT"]),
    //                MinAmount = Convert.ToDecimal(rd["MinAmount"]),
    //                MaxAmount = Convert.ToDecimal(rd["MaxAmount"]),
    //                CommissionPercent = Convert.ToDecimal(rd["CommissionPercent"])
    //            });
    //    }
    //    return list;
    //}
    public async Task Update(TransactionsTypesModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateTypeTransaction", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdTypeTransaction", m.IdTypeTransaction);
        cmd.Parameters.AddWithValue("@Name", m.Name);
        cmd.Parameters.AddWithValue("@Commission", m.Commission ?? 0);
        cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
        cmd.Parameters.AddWithValue("@NumberT", m.NumberT);
        cmd.Parameters.AddWithValue("@UserU", m.UserU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }

    public async Task Delete(int id, string user)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteTypeTransaction", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdTypeTransaction", id);
        cmd.Parameters.AddWithValue("@UserU", user);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }
    public async Task<List<TransactionsTypesModel>> GetAllTypes()
    {
        var list = new List<TransactionsTypesModel>();
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTransactionsTypes", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new TransactionsTypesModel
            {
                IdTypeTransaction = Convert.ToInt32(rd["IdTypeTransaction"]),
                Name = rd["Name"].ToString(),
                Commission = Convert.ToDecimal(rd["Commission"]),
                NumberT = Convert.ToInt32(rd["NumberT"])
            });
        }
        await conn.CloseAsync();
        return list;
    }
    public async Task DeleteCommissionRanges(int numberT)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteTypeTransactionCommissions", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@NumberT", numberT);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}