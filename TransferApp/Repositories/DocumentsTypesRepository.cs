using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class DocumentsTypesRepository
{
    private readonly ApplicationDbContext _db;

    public DocumentsTypesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<DocumentsTypes>> GetAll()
    {
        var list = new List<DocumentsTypes>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetDocumentsTypes", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new DocumentsTypes
            {
                IdDocumentType = (int)rd["IdDocumentType"],
                Name = rd["Name"].ToString(),
                Description = rd["Description"].ToString(),
                Status = (int)rd["Status"]
            });
        }

        return list;
    }

    public async Task<DocumentsTypes> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetDocumentTypeById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@IdDocumentType", id);

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        if (await rd.ReadAsync())
        {
            return new DocumentsTypes
            {
                IdDocumentType = (int)rd["IdDocumentType"],
                Name = rd["Name"].ToString(),
                Description = rd["Description"].ToString(),
                Status = (int)rd["Status"]
            };
        }

        return null;
    }

    public async Task Create(DocumentsTypes m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateDocumentType", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Name", m.Name);
        cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
        cmd.Parameters.AddWithValue("@UserC", m.UserC);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Update(DocumentsTypes m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateDocumentType", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdDocumentType", m.IdDocumentType);
        cmd.Parameters.AddWithValue("@Name", m.Name);
        cmd.Parameters.AddWithValue("@Description", m.Description ?? "");
        cmd.Parameters.AddWithValue("@UserU", m.UserU);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id, string user)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteDocumentType", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@IdDocumentType", id);
        cmd.Parameters.AddWithValue("@UserU", user);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }
}