using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class CountryRepository
{
    private readonly ApplicationDbContext _db;

    public CountryRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<CountriesModel>> GetAll()
    {
        var list = new List<CountriesModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetCountries", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new CountriesModel
            {
                IdCountry = (int)reader["IdCountry"],
                Name = reader["Name"].ToString(),
                Code = reader["Code"].ToString(),
                Currency = reader["Currency"].ToString()
            });
        }

        return list;
    }

    public async Task Create(CountriesModel model)
    {
        try
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_CreateCountry", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Currency", model.Currency);
            cmd.Parameters.AddWithValue("@UserC", model.UserC);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }
        catch (SqlException ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<CountriesModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetCountryById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdCountry", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new CountriesModel
            {
                IdCountry = (int)reader["IdCountry"],
                Name = reader["Name"].ToString(),
                Code = reader["Code"].ToString(),
                Currency = reader["Currency"].ToString()
            };
        }

        return null;
    }

    public async Task Update(CountriesModel model)
    {
        try
        {
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_UpdateCountry", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@IdCountry", model.IdCountry);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Code", model.Code);
            cmd.Parameters.AddWithValue("@Currency", model.Currency);
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
        using var cmd = new SqlCommand("sp_DeleteCountry", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdCountry", id);
        cmd.Parameters.AddWithValue("@UserU", userU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}