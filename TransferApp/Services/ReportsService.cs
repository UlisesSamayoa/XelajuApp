using Microsoft.Playwright;
using TransferApp.Models.Reports;
using TransferApp.Repositories;

namespace TransferApp.Services
{
    public class ReportsService
    {
        private readonly ReportsRepository _repo;
        private readonly IConfiguration _configuration;

        //public ReportsService(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public ReportsService(ReportsRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }

        //REPORTE DE TRANSACCIONES DEL DIA
        public async Task<List<TransactionReportModel>> GetDayliTransactionsReport(DateTime startDate, DateTime endDate)
        {
            return await _repo.GetDayliTransactionsReport(startDate, endDate);
        }
        public async Task<byte[]> GenerateDayliTransactionsReport(DateTime startDate, DateTime endDate)
        {
            var baseUrl = _configuration["ApplicationUrl"];
            string url =
                //$"{urlBase}/Reports/TransactionsPdf" +
                $"{baseUrl}/Reports/DayliTransactionsPdf" +
                $"?startDate={startDate:yyyy-MM-dd}" +
                $"&endDate={endDate:yyyy-MM-dd}" +
                $"&transactionType={"Dayli Transactions"}";
            return await GeneratePdfFromUrl(url);
        }

        //REPORTE DE TRANSACCIONES POR TIPO DE TRANSACCIONES
        public async Task<List<TransactionReportModel>> GetTransactionsReport(DateTime startDate, DateTime endDate, int transactionType)
        {
            return await _repo.GetTransactionsReport(startDate, endDate, transactionType);
        }
        public async Task<byte[]> GenerateTransactionsReport(DateTime startDate, DateTime endDate, int transactionType)
        {
            var baseUrl = _configuration["ApplicationUrl"];
            string url =
                //$"{urlBase}/Reports/TransactionsPdf" +
                $"{baseUrl}/Reports/TransactionsPdf" +
                $"?startDate={startDate:yyyy-MM-dd}" +
                $"&endDate={endDate:yyyy-MM-dd}" +
                $"&transactionType={transactionType}";
            return await GeneratePdfFromUrl(url);
        }

        //REPORTE DE NUEVOS CLIENTES
        //public async Task<List<TransactionReportModel>> GetGenerateNewClientsReport(DateTime startDate, DateTime endDate)
        //{
        //    return await _repo.GetDayliTransactionsReport(startDate, endDate);
        //}
        public async Task<NewClientReportViewModel> GetGenerateNewClientsReport(DateTime startDate, DateTime endDate)
        {
            var clients = await _repo.GetNewClientsReport(startDate, endDate);
            return new NewClientReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                GeneratedDate = DateTime.Now,
                Clients = clients
            };
        }
        public async Task<byte[]> GenerateNewClientsReport(DateTime startDate, DateTime endDate)
        {
            var baseUrl = _configuration["ApplicationUrl"];
            string url =
                $"{baseUrl}/Reports/GenerateNewClientsPdf" +
                $"?startDate={startDate:yyyy-MM-dd}" +
                $"&endDate={endDate:yyyy-MM-dd}" +
                $"&transactionType={"New Clients"}";
            return await GeneratePdfFromUrl(url);
        }

        private async Task<byte[]> GeneratePdfFromUrl(string url)
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync(new BrowserNewPageOptions
            {
                IgnoreHTTPSErrors = true
            });

            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            return await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                DisplayHeaderFooter = true,
                HeaderTemplate = "<div></div>",
                FooterTemplate = @"
                <div style='width:100%;
                font-size:9px;
                padding:0 20px;
                display:flex;
                justify-content:space-between'>
                <span>Money Transfer System</span>
                <span>
                Page <span class='pageNumber'></span>
                of <span class='totalPages'></span>
                </span>
                </div>",

                Margin = new Margin
                {
                    Top = "40px",
                    Bottom = "60px",
                    Left = "20px",
                    Right = "20px"
                }
            });
        }

    }
}
