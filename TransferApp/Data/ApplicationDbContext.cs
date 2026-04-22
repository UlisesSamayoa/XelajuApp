using Microsoft.Data.SqlClient;
namespace TransferApp.Data
{
    //public class ApplicationDbContext : DbContext
    //{
    //    private string Conexion = string.Empty;
    //    bool IsDev = false;
    //    string Frase = string.Empty;
    //    string BaseDeDatos = string.Empty;
    //    public ApplicationDbContext() {
    //        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsetting.json").Build();
    //        Frase = builder.GetSection("Security:CryptoKey").Value;
    //        if (!IsDev)
    //        {
    //            Conexion = builder.GetSection("ConnectionStrings:DefaultConnection").Value;
    //        }
    //        else
    //        {
    //            var encrypted = builder.GetSection("ConnectionStrings:DefaultConnection").Value;

    //            var crypto = new CryptoService(Frase);
    //            Conexion = crypto.Decrypt(encrypted);
    //        }
    //    }
    //    public string getConexion()
    //    {
    //        return Conexion;
    //    }
    //}
    public class ApplicationDbContext
    {
        private readonly string _connectionString;

        public ApplicationDbContext(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
