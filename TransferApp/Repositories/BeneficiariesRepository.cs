using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class BeneficiariesRepository
{
    private readonly ApplicationDbContext _db;

    public BeneficiariesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<BeneficiariesModel>> GetAll()
    {
        var list = new List<BeneficiariesModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetBeneficiaries", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new BeneficiariesModel
            {
                IdBeneficiarie = (int)rd["IdBeneficiarie"],
                FirstName = rd["FirstName"].ToString(),
                LastName = rd["LastName"].ToString(),
                IdDocumentType = rd["IdDocumentType"].ToString(),
                DocumentTypeName = rd["DocumentTypeName"].ToString(),
                DocumentNumber = rd["DocumentNumber"].ToString(),
                Country = rd["Country"].ToString(),
                Status = (int)rd["Status"],
                ParentClientName = rd["ParentClientName"]?.ToString(),
                IdClient_fk = rd["IdClient_fk"] != DBNull.Value ? (int)rd["IdClient_fk"] : 0,
                CountryName = rd["CountryName"].ToString()
            });
        }

        return list;
    }

    public async Task<BeneficiariesModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetBeneficiarieById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdBeneficiarie", id);

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        if (await rd.ReadAsync())
        {
            return new BeneficiariesModel
            {
                IdBeneficiarie = (int)rd["IdBeneficiarie"],
                FirstName = rd["FirstName"].ToString(),
                LastName = rd["LastName"].ToString(),
                IdDocumentType = rd["IdDocumentType"].ToString(),
                DocumentNumber = rd["DocumentNumber"].ToString(),
                Country = rd["Country"].ToString(),
                Status = (int)rd["Status"],
                ParentClientName = rd["ParentClientName"]?.ToString(),
                IdClient_fk = rd["IdClient_fk"] != DBNull.Value ? (int)rd["IdClient_fk"] : 0,
                CountryName = rd["CountryName"].ToString()
            };
        }

        return null;
    }

    public async Task Create(BeneficiariesModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateBeneficiarie", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@FirstName", m.FirstName);
        cmd.Parameters.AddWithValue("@LastName", m.LastName);
        cmd.Parameters.AddWithValue("@IdDocumentType", m.IdDocumentType);
        cmd.Parameters.AddWithValue("@DocumentNumber", m.DocumentNumber);
        cmd.Parameters.AddWithValue("@Country", m.Country);
        cmd.Parameters.AddWithValue("@UserC", m.UserC);
        cmd.Parameters.AddWithValue("@IdClient_fk", m.IdClient_fk);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Update(BeneficiariesModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateBeneficiarie", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdBeneficiarie", m.IdBeneficiarie);
        cmd.Parameters.AddWithValue("@FirstName", m.FirstName);
        cmd.Parameters.AddWithValue("@LastName", m.LastName);
        cmd.Parameters.AddWithValue("@IdDocumentType", m.IdDocumentType);
        cmd.Parameters.AddWithValue("@DocumentNumber", m.DocumentNumber);
        cmd.Parameters.AddWithValue("@Country", m.Country);
        cmd.Parameters.AddWithValue("@UserU", m.UserU);
        cmd.Parameters.AddWithValue("@IdClient_fk", m.IdClient_fk);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id, string user)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteBeneficiarie", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdBeneficiarie", id);
        cmd.Parameters.AddWithValue("@UserU", user);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

   
}