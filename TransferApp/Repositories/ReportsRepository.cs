using Microsoft.Data.SqlClient;
using System.Data;
using TransferApp.Data;
using TransferApp.Models.Reports;

namespace TransferApp.Repositories
{
    public class ReportsRepository
    {
        private readonly ApplicationDbContext _db;

        public ReportsRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        //REPORTE DE TRANSACCIONES DEL DIA
        public async Task<List<TransactionReportModel>> GetDayliTransactionsReport(DateTime startDate, DateTime endDate)
        {
            var list = new List<TransactionReportModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_ReportDayliTransactions", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new TransactionReportModel
                {
                    ReferenceNumber = rd["ReferenceNumber"].ToString(),
                    DateC = Convert.ToDateTime(rd["DateC"]),
                    ClientName = rd["ClientName"].ToString(),
                    CompanyName = rd["CompanyName"].ToString(),
                    Amount = Convert.ToDecimal(rd["Amount"]),
                    Commission = Convert.ToDecimal(rd["Comission"]),
                    FixedCommission = Convert.ToDecimal(rd["FixedCommission"]),
                    TotalCommission = Convert.ToDecimal(rd["TotalCommission"]),
                    TotalAmount = Convert.ToDecimal(rd["TotalAmount"]),
                    CalculationMode = Convert.ToByte(rd["CalculationMode"])
                });
            }
            return list;
        }

        //REPORTE DE TRANSACCIONES POR TIPO DE TRANSACCION
        public async Task<List<TransactionReportModel>> GetTransactionsReport(DateTime startDate, DateTime endDate, int transactionType)
        {
            var list = new List<TransactionReportModel>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_ReportTransactions", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            cmd.Parameters.AddWithValue("@TransactionType", transactionType);
            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new TransactionReportModel
                {
                    ReferenceNumber = rd["ReferenceNumber"].ToString(),
                    DateC = Convert.ToDateTime(rd["DateC"]),
                    ClientName = rd["ClientName"].ToString(),
                    CompanyName = rd["CompanyName"].ToString(),
                    Amount = Convert.ToDecimal(rd["Amount"]),
                    Commission = Convert.ToDecimal(rd["Comission"]),
                    FixedCommission = Convert.ToDecimal(rd["FixedCommission"]),
                    TotalCommission = Convert.ToDecimal(rd["TotalCommission"]),
                    TotalAmount = Convert.ToDecimal(rd["TotalAmount"]),
                    CalculationMode = Convert.ToByte(rd["CalculationMode"])
                });
            }
            return list;
        }
        //REPORTE DE CLIENTES NUEVOS
        public async Task<List<NewClientReportViewModel.ClientItem>> GetNewClientsReport(DateTime startDate, DateTime endDate)
        {
            var list = new List<NewClientReportViewModel.ClientItem>();
            using var conn = _db.CreateConnection();
            using var cmd = new SqlCommand("sp_GetClientsReport", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            await conn.OpenAsync();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new NewClientReportViewModel.ClientItem
                {
                    DocumentNumber = rd["DocumentNumber"].ToString(),
                    FullName = rd["FullName"].ToString(),
                    Country = rd["Country"].ToString(),
                    Phone = rd["Phone"].ToString(),
                    Address = rd["Address"].ToString(),
                    //IssueDate = Convert.ToDateTime(rd["IssueDate"]),
                    //ExpirationDate = Convert.ToDateTime(rd["ExpirationDate"]),
                    IssueDate = rd["IssueDate"] == DBNull.Value ? null : Convert.ToDateTime(rd["IssueDate"]),
                    ExpirationDate = rd["ExpirationDate"] == DBNull.Value ? null : Convert.ToDateTime(rd["ExpirationDate"]),
                    Dob = rd["Dob"] == DBNull.Value ? null : Convert.ToDateTime(rd["Dob"]),
                    //Dob = Convert.ToDateTime(rd["Dob"]),
                    Status = Convert.ToInt32(rd["Status"]),
                    DateC = Convert.ToDateTime(rd["DateC"])
                });
            }
            return list;
        }

    }
}
