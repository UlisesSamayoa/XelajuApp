using Microsoft.Playwright;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using TransferApp.Models.Reports;
using TransferApp.Repositories;
using TransferApp.ViewModels;

namespace TransferApp.Services
{
    public class ReportsService
    {
        private readonly ReportsRepository _repo;
        private readonly ClientsRepository _clientRepo;
        private readonly TransactionsService _Transactionservice;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TransactionAttachmentRepository _repoAttach;
        //public ReportsService(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public ReportsService(ReportsRepository repo, ClientsRepository clientRepo, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, TransactionsService transactionservice, TransactionAttachmentRepository repoAttach)
        {
            _repo = repo;
            _clientRepo = clientRepo;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _Transactionservice = transactionservice;
            _repoAttach = repoAttach;
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

        //REPORTE DE TRANSACCIONES POR CLIENTE
        //public async Task<List<TransactionReportModel>> GetClientTransactionsReport(DateTime startDate, DateTime endDate, int client_Id)
        //{
        //    return await _repo.GetClientTransactionsReport(startDate, endDate, client_Id);
        //}
        public async Task<ClientTransactionsReportViewModel> GetClientTransactionsReport(DateTime startDate, DateTime endDate, int clientId)
        {
            // Obtener cliente
            var client = await _clientRepo.GetById(clientId);

            // Obtener transacciones
            var transactions = await _repo.GetClientTransactionsReport(
                startDate,
                endDate,
                clientId);

            // Construir ViewModel
            return new ClientTransactionsReportViewModel
            {
                Client = client,
                Transactions = transactions,
                StartDate = startDate,
                EndDate = endDate,
                TotalTransactions = transactions.Count,
                TotalAmount = transactions.Sum(x => x.Amount),
                TotalCommission = transactions.Sum(x => x.TotalCommission),
                GeneratedDate = DateTime.Now,
            };
        }
        public async Task<byte[]> GenerateClientTransactionsReport(DateTime startDate, DateTime endDate, int client_Id)
        {
            var baseUrl = _configuration["ApplicationUrl"];
            string url =
                //$"{urlBase}/Reports/TransactionsPdf" +
                $"{baseUrl}/Reports/ClientTransactionsPdf" +
                $"?startDate={startDate:yyyy-MM-dd}" +
                $"&endDate={endDate:yyyy-MM-dd}" +
                $"&client_Id={client_Id}";
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

        //reporte de transacciones
        public async Task<TransactionPreviewViewModel> GetTransactionPreview(int id)
        {
            return await _Transactionservice.GetTransactionPreview(id);
        }
        //MONEY TRANSFER
        //public async Task<byte[]> GenerateMoneyTransferPreview(int id)
        //{
        //    var baseUrl = _configuration["ApplicationUrl"];

        //    string url =
        //        $"{baseUrl}/Reports/MoneyTransferPreviewPdf?id={id}";

        //    return await GeneratePreviewPdfFromUrl(url);
        //}
        public async Task<byte[]> GenerateMoneyTransferPreview(int id)
        {
            var baseUrl = _configuration["ApplicationUrl"];

            string url = $"{baseUrl}/Reports/MoneyTransferPreviewPdf?id={id}";
            var reportPdf = await GeneratePreviewPdfFromUrl(url);
            var attachments = await _repoAttach.GetAttachments(id);
            var pdfFiles = attachments.Where(x => x.FileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase) && File.Exists(x.FilePath)).Select(x => x.FilePath).ToList();
            if (!pdfFiles.Any())
                return reportPdf;
            return MergePdfs(reportPdf, pdfFiles);
        }
        //public async Task<byte[]> GenerateMoneyTransferPreview(int id)
        //{
        //    var baseUrl = _configuration["ApplicationUrl"];
        //    string url = $"{baseUrl}/Reports/MoneyTransferPreviewPdf?id={id}";
        //    var pdf = await GeneratePreviewPdfFromUrl(url);

        //    var attachments = await _repoAttach.GetAttachments(id);


        //    var pdfFiles = attachments
        //        .Where(x => x.FileExtension == ".pdf")
        //        .Select(x => x.FilePath)
        //        .ToList();
        //    return MergePdfs(pdf, pdfFiles);
        //}
        // CHECK
        public async Task<byte[]> GenerateCheckPreview(int id)
        {
            var baseUrl = _configuration["ApplicationUrl"];

            string url = $"{baseUrl}/Reports/CheckPreviewPdf?id={id}";
            var reportPdf = await GeneratePreviewPdfFromUrl(url);

            var attachments = await _repoAttach.GetAttachments(id);

            var pdfFiles = attachments
                .Where(x => x.FileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                         && File.Exists(x.FilePath))
                .Select(x => x.FilePath)
                .ToList();

            if (!pdfFiles.Any())
                return reportPdf;

            return MergePdfs(reportPdf, pdfFiles);
        }

        // DOMESTIC
        public async Task<byte[]> GenerateDomesticPreview(int id)
        {
            var baseUrl = _configuration["ApplicationUrl"];

            string url = $"{baseUrl}/Reports/DomesticPreviewPdf?id={id}";
            var reportPdf = await GeneratePreviewPdfFromUrl(url);

            var attachments = await _repoAttach.GetAttachments(id);

            var pdfFiles = attachments
                .Where(x => x.FileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                         && File.Exists(x.FilePath))
                .Select(x => x.FilePath)
                .ToList();

            if (!pdfFiles.Any())
                return reportPdf;

            return MergePdfs(reportPdf, pdfFiles);
        }

        // MONEY ORDER
        public async Task<byte[]> GenerateMoneyOrderPreview(int id)
        {
            var baseUrl = _configuration["ApplicationUrl"];

            string url = $"{baseUrl}/Reports/MoneyOrderPreviewPdf?id={id}";
            var reportPdf = await GeneratePreviewPdfFromUrl(url);

            var attachments = await _repoAttach.GetAttachments(id);

            var pdfFiles = attachments
                .Where(x => x.FileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                         && File.Exists(x.FilePath))
                .Select(x => x.FilePath)
                .ToList();

            if (!pdfFiles.Any())
                return reportPdf;

            return MergePdfs(reportPdf, pdfFiles);
        }

        // PAID SERVICE
        public async Task<byte[]> GeneratePaidServicePreview(int id)
        {
            var baseUrl = _configuration["ApplicationUrl"];

            string url = $"{baseUrl}/Reports/PaidServicePreviewPdf?id={id}";
            var reportPdf = await GeneratePreviewPdfFromUrl(url);

            var attachments = await _repoAttach.GetAttachments(id);

            var pdfFiles = attachments
                .Where(x => x.FileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase)
                         && File.Exists(x.FilePath))
                .Select(x => x.FilePath)
                .ToList();

            if (!pdfFiles.Any())
                return reportPdf;

            return MergePdfs(reportPdf, pdfFiles);
        }
        ////CHEQUES
        //public async Task<byte[]> GenerateCheckPreview(int id)
        //{
        //    var baseUrl = _configuration["ApplicationUrl"];

        //    string url =
        //        $"{baseUrl}/Reports/CheckPreviewPdf?id={id}";

        //    return await GeneratePreviewPdfFromUrl(url);
        //}
        ////DOMESTIC
        //public async Task<byte[]> GenerateDomesticPreview(int id)
        //{
        //    var baseUrl = _configuration["ApplicationUrl"];

        //    string url =
        //        $"{baseUrl}/Reports/DomesticPreviewPdf?id={id}";

        //    return await GeneratePreviewPdfFromUrl(url);
        //}
        ////MONEY ORDER
        //public async Task<byte[]> GenerateMoneyOrderPreview(int id)
        //{
        //    var baseUrl = _configuration["ApplicationUrl"];

        //    string url =
        //        $"{baseUrl}/Reports/MoneyOrderPreviewPdf?id={id}";

        //    return await GeneratePreviewPdfFromUrl(url);
        //}
        ////PAID SERVICE
        //public async Task<byte[]> GeneratePaidServicePreview(int id)
        //{
        //    var baseUrl = _configuration["ApplicationUrl"];

        //    string url =
        //        $"{baseUrl}/Reports/PaidServicePreviewPdf?id={id}";

        //    return await GeneratePreviewPdfFromUrl(url);
        //}


        //Reporte de transacciones


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
                <span>Xalaju Water</span>
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

        private async Task<byte[]> GeneratePreviewPdfFromUrl(string url)
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

            var page = await browser.NewPageAsync(
                new BrowserNewPageOptions
                {
                    IgnoreHTTPSErrors = true,
                    ViewportSize = new ViewportSize
                    {
                        Width = 1400,
                        Height = 1000
                    }
                });

            await page.GotoAsync(url,
                new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.NetworkIdle
                });
            var images = await page.Locator("img").CountAsync();

            Console.WriteLine($"Images found: {images}");

            for (int i = 0; i < images; i++)
            {
                var src = await page.Locator("img").Nth(i).GetAttributeAsync("src");

                Console.WriteLine($"Image {i}: {src}");
            }
            await page.EvaluateAsync(@"
                async () => {
                    const images = Array.from(document.images);

                    await Promise.all(
                        images.map(img => {
                            if (img.complete) return Promise.resolve();

                            return new Promise(resolve => {
                                img.onload = resolve;
                                img.onerror = resolve;
                            });
                        })
                    );
                }
            ");
            return await page.PdfAsync(new PagePdfOptions
            {
                Format = "A4",
                PrintBackground = true,
                Margin = new Margin
                {
                    Top = "20px",
                    Bottom = "40px",
                    Left = "30px",
                    Right = "30px"
                }

            });
        }


        public byte[] MergePdfs(
    byte[] mainPdf,
    List<string> pdfFiles)
        {
            using var output = new PdfDocument();


            using var mainStream = new MemoryStream(mainPdf);

            var main = PdfReader.Open(
                mainStream,
                PdfDocumentOpenMode.Import);


            foreach (var page in main.Pages)
            {
                output.AddPage(page);
            }


            foreach (var file in pdfFiles)
            {
                var attachment = PdfReader.Open(
                    file,
                    PdfDocumentOpenMode.Import);

                foreach (var page in attachment.Pages)
                {
                    output.AddPage(page);
                }
            }


            using var result = new MemoryStream();

            output.Save(result);

            return result.ToArray();
        }


    }
}
