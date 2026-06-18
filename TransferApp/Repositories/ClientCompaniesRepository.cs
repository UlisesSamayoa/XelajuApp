using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class ClientCompaniesRepository
{
    private readonly ApplicationDbContext _db;

    public ClientCompaniesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ClientCompaniesModel>> GetAll()
    {
        var list = new List<ClientCompaniesModel>();
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetClientCompanies", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            list.Add(new ClientCompaniesModel
            {
                IdClientCompany = Convert.ToInt32(rd["IdClientCompany"]),
                IdClient = Convert.ToInt32(rd["IdClient"]),
                ClientName = rd["ClientName"]?.ToString(),
                IdCompany = Convert.ToInt32(rd["IdCompany"]),
                CompanyName = rd["CompanyName"]?.ToString(),
                AccountNumber = rd["AccountNumber"]?.ToString(),
                Status = Convert.ToInt32(rd["Status"])
            });
        }
        await conn.CloseAsync();
        return list;
    }
    public async Task<int> Create(ClientCompaniesModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateClientCompany", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdClient", m.IdClient);
        cmd.Parameters.AddWithValue("@IdCompany", m.IdCompany);
        cmd.Parameters.AddWithValue("@AccountNumber", m.AccountNumber);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        await conn.CloseAsync();
        return Convert.ToInt32(result);
    }
    public async Task Delete(int id, string user)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteClientCompany", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdClientCompany", id);
        cmd.Parameters.AddWithValue("@UserU", user);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }
}