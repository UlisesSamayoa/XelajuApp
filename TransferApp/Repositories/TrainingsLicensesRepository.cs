using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class TrainingsLicensesRepository
{
    private readonly ApplicationDbContext _db;

    public TrainingsLicensesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<TrainingsLicensesModel>> GetAll()
    {
        var list = new List<TrainingsLicensesModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTrainingLicenses", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new TrainingsLicensesModel
            {
                IdTrainingsLicenses = (int)reader["IdTrainingsLicenses"],
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                TrainingsLicensesFile = reader["TrainingsLicensesFile"].ToString(),
                Status = (int)reader["Status"]
            });
        }
        await conn.CloseAsync();
        return list;
    }

    public async Task<TrainingsLicensesModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetTrainingLicenseById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new TrainingsLicensesModel
            {
                IdTrainingsLicenses = (int)reader["IdTrainingsLicenses"],
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                TrainingsLicensesFile = reader["TrainingsLicensesFile"].ToString(),
                Status = (int)reader["Status"]
            };
        }
        await conn.CloseAsync();
        return null;
    }

    public async Task Create(TrainingsLicensesModel model)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateTrainingLicense", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@File", model.TrainingsLicensesFile ?? "");
        cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
        cmd.Parameters.AddWithValue("@UserC", model.UserC);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }

    public async Task Update(TrainingsLicensesModel model)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateTrainingLicense", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", model.IdTrainingsLicenses);
        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@File", model.TrainingsLicensesFile ?? "");
        cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
        cmd.Parameters.AddWithValue("@UserU", model.UserU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }

    public async Task Delete(int id, string userU)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteTrainingLicense", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserU", userU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        await conn.CloseAsync();
    }
}