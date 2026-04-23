using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class ReviewsRepository
{
    private readonly ApplicationDbContext _db;

    public ReviewsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReviewsModel>> GetAll()
    {
        var list = new List<ReviewsModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetReviews", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new ReviewsModel
            {
                IdReview = (int)reader["IdReview"],
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                ReviewFile = reader["ReviewFile"].ToString(),
                Status = (int)reader["Status"]
            });
        }

        return list;
    }

    public async Task<ReviewsModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetReviewById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new ReviewsModel
            {
                IdReview = (int)reader["IdReview"],
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                ReviewFile = reader["ReviewFile"].ToString(),
                Status = (int)reader["Status"]
            };
        }

        return null;
    }

    public async Task Create(ReviewsModel model)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateReview", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@File", model.ReviewFile ?? "");
        cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
        cmd.Parameters.AddWithValue("@UserC", model.UserC);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Update(ReviewsModel model)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateReview", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", model.IdReview);
        cmd.Parameters.AddWithValue("@Name", model.Name);
        cmd.Parameters.AddWithValue("@File", model.ReviewFile ?? "");
        cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
        cmd.Parameters.AddWithValue("@UserU", model.UserU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id, string userU)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteReview", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserU", userU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}