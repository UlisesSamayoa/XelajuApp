using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models;

public class ClientsRepository
{
    private readonly ApplicationDbContext _db;

    public ClientsRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ClientsModel>> GetAll()
    {
        var list = new List<ClientsModel>();
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetClients", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(Map(reader));
        }
        //var schemaTable = reader.GetSchemaTable();
        //foreach (DataRow row in schemaTable.Rows)
        //{
        //    Console.WriteLine(row["ColumnName"]);
        //}
        //while (await reader.ReadAsync())
        //{
        //    var client = new ClientsModel
        //    {
        //        IdClient = reader["IdClient"] != DBNull.Value ? Convert.ToInt32(reader["IdClient"]) : 0,
        //        FirstName = reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : string.Empty,
        //        LastName = reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : string.Empty,
        //        IdDocumentType = reader["IdDocumentType"] != DBNull.Value ? reader["IdDocumentType"].ToString() : string.Empty,
        //        DocumentNumber = reader["DocumentNumber"] != DBNull.Value ? reader["DocumentNumber"].ToString() : string.Empty,
        //        Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty,
        //        ExpirationDate = reader["ExpirationDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpirationDate"]) : DateTime.MinValue,
        //        Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty,
        //        Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
        //        Picture = reader["Picture"] != DBNull.Value ? reader["Picture"].ToString() : string.Empty,
        //        Status = reader["Status"] != DBNull.Value ? Convert.ToInt32(reader["Status"]) : 0,
        //        DateC = reader["DateC"] != DBNull.Value ? Convert.ToDateTime(reader["DateC"]) : DateTime.MinValue,
        //        UserC = reader["UserC"] != DBNull.Value ? reader["UserC"].ToString() : string.Empty,
        //        DateU = reader["DateU"] != DBNull.Value ? Convert.ToDateTime(reader["DateU"]) : DateTime.MinValue,
        //        UserU = reader["UserU"] != DBNull.Value ? reader["UserU"].ToString() : string.Empty,
        //        CountryName = reader["CountryName"] != DBNull.Value ? reader["CountryName"].ToString() : string.Empty
        //    };
        //    list.Add(client);
        //}
        return list;
    }


    public async Task<ClientsModel> GetById(int id)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_GetClientById", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ReadAsync() ? Map(reader) : null;
    }

    //public async Task Create(ClientsModel model)
    //{
    //    using var conn = _db.CreateConnection();
    //    using var cmd = new SqlCommand("sp_CreateClient", conn);

    //    cmd.CommandType = CommandType.StoredProcedure;

    //    AddParams(cmd, model, true);

    //    await conn.OpenAsync();
    //    await cmd.ExecuteNonQueryAsync();
    //}
    public async Task<int> Create(ClientsModel m)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_CreateClient", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        AddParams(cmd, m, true);

        await conn.OpenAsync();

        var result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task Update(ClientsModel model)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_UpdateClient", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@Id", model.IdClient);
        AddParams(cmd, model, false);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Delete(int id, string user)
    {
        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_DeleteClient", conn);

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserU", user);
        cmd.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    //private ClientsModel Map(SqlDataReader r) => new()
    //{
    //    IdClient = (int)r["IdClient"],
    //    FirstName = r["FirstName"].ToString(),
    //    LastName = r["LastName"].ToString(),
    //    IdDocumentType = r["IdDocumentType"].ToString(),
    //    DocumentNumber = r["DocumentNumber"].ToString(),
    //    Address = r["Address"].ToString(),
    //    Phone = r["Phone"].ToString(),
    //    Country = r["Country"].ToString(),
    //    Picture = r["Picture"].ToString(),
    //    Status = (int)r["Status"],
    //    ExpirationDate = r["ExpirationDate"] != DBNull.Value ? Convert.ToDateTime(r["ExpirationDate"]) : DateTime.MinValue,
    //};
    private ClientsModel Map(SqlDataReader reader)
    {
        return new ClientsModel
        {
            IdClient = reader["IdClient"] != DBNull.Value ? Convert.ToInt32(reader["IdClient"]) : 0,
            FirstName = reader["FirstName"] != DBNull.Value ? reader["FirstName"].ToString() : string.Empty,
            LastName = reader["LastName"] != DBNull.Value ? reader["LastName"].ToString() : string.Empty,
            IdDocumentType = reader["IdDocumentType"] != DBNull.Value ? reader["IdDocumentType"].ToString() : string.Empty,
            DocumentNumber = reader["DocumentNumber"] != DBNull.Value ? reader["DocumentNumber"].ToString() : string.Empty,
            Address = reader["Address"] != DBNull.Value ? reader["Address"].ToString() : string.Empty,
            ExpirationDate = reader["ExpirationDate"] != DBNull.Value ? Convert.ToDateTime(reader["ExpirationDate"]) : DateTime.MinValue,
            Phone = reader["Phone"] != DBNull.Value ? reader["Phone"].ToString() : string.Empty,
            Country = reader["Country"] != DBNull.Value ? reader["Country"].ToString() : string.Empty,
            Picture = reader["Picture"] != DBNull.Value ? reader["Picture"].ToString() : string.Empty,
            Status = reader["Status"] != DBNull.Value ? Convert.ToInt32(reader["Status"]) : 0,
            DateC = reader["DateC"] != DBNull.Value ? Convert.ToDateTime(reader["DateC"]) : DateTime.MinValue,
            UserC = reader["UserC"] != DBNull.Value ? reader["UserC"].ToString() : string.Empty,
            DateU = reader["DateU"] != DBNull.Value ? Convert.ToDateTime(reader["DateU"]) : DateTime.MinValue,
            UserU = reader["UserU"] != DBNull.Value ? reader["UserU"].ToString() : string.Empty,
            CountryName = reader["CountryName"] != DBNull.Value ? reader["CountryName"].ToString() : string.Empty
        };
    }

    private void AddParams(SqlCommand cmd, ClientsModel m, bool isCreate)
    {
        cmd.Parameters.AddWithValue("@FirstName", m.FirstName);
        cmd.Parameters.AddWithValue("@LastName", m.LastName);
        cmd.Parameters.AddWithValue("@IdDocumentType", m.IdDocumentType);
        cmd.Parameters.AddWithValue("@DocumentNumber", m.DocumentNumber);
        cmd.Parameters.AddWithValue("@Address", m.Address ?? "");
        cmd.Parameters.AddWithValue("@ExpirationDate", m.ExpirationDate);
        cmd.Parameters.AddWithValue("@Phone", m.Phone ?? "");
        cmd.Parameters.AddWithValue("@Country", m.Country);
        cmd.Parameters.AddWithValue("@Picture", m.Picture ?? "");

        if (isCreate)
            cmd.Parameters.AddWithValue("@UserC", m.UserC);
        else
            cmd.Parameters.AddWithValue("@UserU", m.UserU);
    }
    //public async Task<List<ClientsModel>> Search(string term)
    //{
    //    var list = new List<ClientsModel>();

    //    using var conn = _db.CreateConnection();
    //    using var cmd = new SqlCommand("sp_SearchClients", conn);

    //    cmd.CommandType = CommandType.StoredProcedure;
    //    cmd.Parameters.AddWithValue("@term", term);

    //    await conn.OpenAsync();
    //    using var rd = await cmd.ExecuteReaderAsync();

    //    while (await rd.ReadAsync())
    //    {
    //        list.Add(new ClientsModel
    //        {
    //            IdClient = (int)rd["IdClient"],
    //            FirstName = rd["FirstName"].ToString(),
    //            LastName = rd["LastName"].ToString(),
    //            DocumentNumber = rd["DocumentNumber"].ToString(),
    //            Phone = rd["Phone"].ToString(),
    //            FullName = rd["FullName"].ToString()
    //        });
    //    }

    //    return list;
    //}
    public async Task<List<ClientsModel>> Search(string term)
    {
        var list = new List<ClientsModel>();

        using var conn = _db.CreateConnection();
        using var cmd = new SqlCommand("sp_SearchClients", conn);

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Term", term ?? "");

        await conn.OpenAsync();
        using var rd = await cmd.ExecuteReaderAsync();

        while (await rd.ReadAsync())
        {
            list.Add(new ClientsModel
            {
                IdClient = (int)rd["IdClient"],
                FirstName = rd["FirstName"].ToString(),
                LastName = rd["LastName"].ToString(),
                DocumentNumber = rd["DocumentNumber"].ToString(),
                Phone = rd["Phone"].ToString(),
                Address = rd["Address"].ToString(),
                IdDocumentType = rd["IdDocumentType"].ToString(),
                Country = rd["Country"].ToString(),
                FullName = rd["FirstName"].ToString() + ' ' + rd["LastName"].ToString()
            });
        }

        return list;
    }
}