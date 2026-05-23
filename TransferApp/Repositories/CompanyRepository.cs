using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class CompanyRepository
{
    private readonly ApplicationDbContext _db;

    public CompanyRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task Create(CompaniesModel model)
    {
        try
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_CreateCompany", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@SwiftCode", model.SwiftCode);
            cmd.Parameters.AddWithValue("@Country", model.Country);
            cmd.Parameters.AddWithValue("@IdCountry", model.IdCountry);
            cmd.Parameters.AddWithValue("@Phone", model.Phone);
            cmd.Parameters.AddWithValue("@ContactPerson", model.ContactPerson);
            cmd.Parameters.AddWithValue("@PhoneContactPerson", model.PhoneContactPerson);
            cmd.Parameters.AddWithValue("@Position", model.Position);
            cmd.Parameters.AddWithValue("@TransactionType", model.TransactionType);
            cmd.Parameters.AddWithValue("@UserC", model.UserC);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<CompaniesModel>> GetAll()
    {
        var list = new List<CompaniesModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetCompanies", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new CompaniesModel
            {
                IdCompany = (int)reader["IdCompany"],
                Name = reader["Name"].ToString(),
                SwiftCode = reader["SwiftCode"].ToString(),
                Country = reader["Country"].ToString(),
                IdCountry = reader["IdCountry"].ToString(),
                Phone = reader["Phone"].ToString(),
                ContactPerson = reader["ContactPerson"].ToString(),
                PhoneContactPerson = reader["PhoneContactPerson"].ToString(),
                Position = reader["Position"].ToString(),
                TransactionType = (int)reader["TransactionType"],
                TransactionTypeName = reader["TransactionTypeName"].ToString(),
                Status = (int)reader["Status"]
            });
        }

        return list;
    }
    public async Task<CompaniesModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetCompanyById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdCompany", id);

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new CompaniesModel
            {
                IdCompany = (int)reader["IdCompany"],
                Name = reader["Name"].ToString(),
                Country = reader["Country"].ToString(),
                IdCountry = reader["IdCountry"].ToString(),
                SwiftCode = reader["SwiftCode"].ToString(),
                Phone = reader["Phone"].ToString(),
                TransactionType = (int)reader["TransactionType"],
                TransactionTypeName = reader["TransactionTypeName"].ToString(),
                ContactPerson = reader["ContactPerson"].ToString(),
                PhoneContactPerson = reader["PhoneContactPerson"].ToString(),
                Position = reader["Position"].ToString()
            };
        }

        return null;
    }
    public async Task Update(CompaniesModel model)
    {
        try
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_UpdateCompany", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@IdCompany", model.IdCompany);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@SwiftCode", model.SwiftCode);
            cmd.Parameters.AddWithValue("@Country", model.Country);
            cmd.Parameters.AddWithValue("@IdCountry", model.IdCountry);
            cmd.Parameters.AddWithValue("@TransactionType", model.TransactionType);
            cmd.Parameters.AddWithValue("@Phone", model.Phone);
            cmd.Parameters.AddWithValue("@ContactPerson", model.ContactPerson);
            cmd.Parameters.AddWithValue("@PhoneContactPerson", model.PhoneContactPerson);
            cmd.Parameters.AddWithValue("@Position", model.Position);
            cmd.Parameters.AddWithValue("@UserU", model.UserU);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task Delete(int id, string userU)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteCompany", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdCompany", id);
        cmd.Parameters.AddWithValue("@UserU", userU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<List<CompaniesModel>> GetByCountry(int countryId)
    {
        var list = new List<CompaniesModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetCompaniesByCountry", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdCountry", countryId);

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new CompaniesModel
            {
                IdCompany = (int)rd["IdCompany"],
                Name = rd["Name"].ToString()
            });
        }

        return list;
    }

    public async Task<List<CompaniesModel>> GetByTransactionType(int transactionType)
    {
        List<CompaniesModel> list = new();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetCompaniesByTransactionType", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue(
            "@TransactionType",
            transactionType
        );

        await conn.OpenAsync();

        using SqlDataReader dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            list.Add(new CompaniesModel
            {
                IdCompany = Convert.ToInt32(dr["IdCompany"]),
                Name = dr["Name"].ToString()
            });
        }


        return list;
    }

}