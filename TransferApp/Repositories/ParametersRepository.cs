using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;
using TransferApp.ViewModels;

public class ParametersRepository
{
    private readonly ApplicationDbContext _db;

    public ParametersRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ParametersModel>> GetAll()
    {
        var list = new List<ParametersModel>();
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetParameters", conn);
        cmd.CommandType =CommandType.StoredProcedure;
        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            list.Add(new ParametersModel
            {
                IdParameters =(int)rd["IdParameters"],
                LastMonth =(bool)rd["LastMonth"],
                CountDays =(int)rd["CountDays"],
                MaxTransactions = (int)rd["MaxTransactions"],
                MaxAmount =(decimal)rd["MaxAmount"],
                Status =(int)rd["Status"]
            });
        }
        return list;
    }

    public async Task<ParametersModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd =new SqlCommand("sp_GetParametersById",conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdParameters",id);
        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();
        if (await rd.ReadAsync())
        {
            return new ParametersModel
            {
                IdParameters =(int)rd["IdParameters"],
                LastMonth =(bool)rd["LastMonth"],
                CountDays =(int)rd["CountDays"],
                MaxTransactions = (int)rd["MaxTransactions"],
                MaxAmount =(decimal)rd["MaxAmount"],
                Status =(int)rd["Status"]
            };
        }
        return null;
    }

    public async Task<int> Create(ParametersModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateParameters",conn);
        cmd.CommandType =CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@LastMonth",m.LastMonth);
        cmd.Parameters.AddWithValue("@CountDays",m.CountDays);
        cmd.Parameters.AddWithValue("@MaxTransactions", m.MaxTransactions);
        cmd.Parameters.AddWithValue("@MaxAmount",m.MaxAmount);
        cmd.Parameters.AddWithValue("@UserC",m.UserC);
        await conn.OpenAsync();
        var result =await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task Update(ParametersModel m)
    {
        using var conn = _db.CreateConnection();

        using var cmd = new SqlCommand("sp_UpdateParameters",conn);
        cmd.CommandType =CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdParameters",m.IdParameters);
        cmd.Parameters.AddWithValue("@LastMonth",m.LastMonth);
        cmd.Parameters.AddWithValue("@CountDays",m.CountDays);
        cmd.Parameters.AddWithValue("@MaxTransactions", m.MaxTransactions);
        cmd.Parameters.AddWithValue("@MaxAmount",m.MaxAmount);
        cmd.Parameters.AddWithValue("@Status",m.Status);
        cmd.Parameters.AddWithValue("@UserU",m.UserU);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id, string user)
    {
        using var conn = _db.CreateConnection();
        using var cmd =new SqlCommand("sp_DeleteParameters",conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdParameters",id);
        cmd.Parameters.AddWithValue("@UserU",user);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<TransactionValidationModel>
    ValidateClientTransactions(string documentNumber)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_ValidateClientTransactions",conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdClient", documentNumber);
        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        if (await rd.ReadAsync())
        {
            return new TransactionValidationModel
            {
                //TotalTransactions = Convert.ToInt32(rd["TotalTransactions"]),
                //MaxAmount = Convert.ToDecimal(rd["MaxAmount"]),
                //MaxTransactions = Convert.ToInt32(rd["MaxTransactions"]),
                //TotalAmount = Convert.ToDecimal(rd["TotalAmount"])
                TotalTransactions = rd["TotalTransactions"] != DBNull.Value? Convert.ToInt32(rd["TotalTransactions"]): 0,
                TotalAmount = rd["TotalAmount"] != DBNull.Value? Convert.ToDecimal(rd["TotalAmount"]): 0,   
                MaxAmount = rd["MaxAmount"] != DBNull.Value? Convert.ToDecimal(rd["MaxAmount"]): 0,
                MaxTransactions = rd["MaxTransactions"] != DBNull.Value? Convert.ToInt32(rd["MaxTransactions"]): 0
            };
            
        }

        return new TransactionValidationModel
        {
            TotalTransactions = 0,
            TotalAmount = 0
        };
    }
}