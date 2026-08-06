using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransferApp.Models.Reports;
using TransferApp.Services;

namespace TransferApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ReportsService _service;
        public ReportsController(ReportsService service)
        {
            _service = service;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TransactionsReport(DateTime startDate, DateTime endDate, int transactionType)
        {
            var data = await _service.GetTransactionsReport(startDate, endDate, transactionType);
            return Json(data);
        }
        //REPORTE DE TRANSACCIONES DEL DIA

        [AllowAnonymous]
        public async Task<IActionResult> GenerateDayliTransactionsReport(DateTime startDate, DateTime endDate)
        {
            byte[] pdf = await _service.GenerateDayliTransactionsReport(startDate, endDate);
            return File(pdf, "application/pdf", $"Transactions_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DayliTransactionsPdf(DateTime startDate, DateTime endDate, int transactionType)
        {
            var transactions = await _service.GetDayliTransactionsReport(startDate, endDate);
            var model = new TransactionReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                GeneratedDate = DateTime.Now,
                TransactionTypeName = "TIPO TRANSACCION",
                Transactions = transactions
            };

            return View("DayliTransactionsReport", model);
        }
        //REPORTE DE TRANSACCIONES POR TIPO DE TRANSACCION

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateTransactionsReport(DateTime startDate, DateTime endDate, int transactionType)
        {
            byte[] pdf = await _service.GenerateTransactionsReport(startDate, endDate, transactionType);
            return File(pdf, "application/pdf", $"Transactions_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> TransactionsPdf(DateTime startDate, DateTime endDate, int transactionType)
        {
            var transactions = await _service.GetTransactionsReport(startDate, endDate, transactionType);
            var model = new TransactionReportViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                GeneratedDate = DateTime.Now,
                TransactionTypeName = "TIPO TRANSACCION",
                Transactions = transactions
            };

            return View("TransactionsReport", model);
        }
        //REPORTE DE TRANSACCIONES POR CLIENTE

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateClientTransactionsReport(DateTime startDate, DateTime endDate, int client_Id)
        {
            byte[] pdf = await _service.GenerateClientTransactionsReport(startDate, endDate, client_Id);
            return File(pdf, "application/pdf", $"Transactions_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ClientTransactionsPdf(DateTime startDate, DateTime endDate, int client_Id)
        {
            var model = await _service.GetClientTransactionsReport(
                startDate,
                endDate,
                client_Id);

            return View("ClientsTransactionsReport", model);
        }

        //REPORTE DE CLIENTES NUEVOS 
        [AllowAnonymous]
        public async Task<IActionResult> GenerateNewClientsReport(DateTime startDate, DateTime endDate)
        {
            byte[] pdf = await _service.GenerateNewClientsReport(startDate, endDate);
            return File(pdf, "application/pdf", $"Transactions_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateNewClientsPdf(DateTime startDate, DateTime endDate)
        {
            var model = await _service.GetGenerateNewClientsReport(startDate, endDate);
            return View("NewClientsReport", model);
        }

        //orquestador de reportes pot transaccion
        [HttpGet]
        public async Task<IActionResult> GenerateTransactionPreview(int id, int type)
        {
            return type switch
            {
                1 => await GenerateCheckPreview(id),
                2 => await GenerateMoneyOrderPreview(id),
                3 => await GenerateMoneyTransferPreview(id),
                4 => await GeneratePaidServicePreview(id),
                5 => await GenerateDomesticPreview(id),

                _ => BadRequest("Invalid transaction type.")
            };
        }

        //REPORTE DE MONEY TRANSFER
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MoneyTransferPreviewPdf(int id)
        {
            var model = await _service.GetTransactionPreview(id);

            return View("Preview/_MoneyTransfer", model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GenerateMoneyTransferPreview(int id)
        {
            byte[] pdf = await _service.GenerateMoneyTransferPreview(id);

            return File(
                pdf,
                "application/pdf",
                $"MoneyTransfer_{id}.pdf");
        }

        //REPORTE DE CHEQUES 
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CheckPreviewPdf(int id)
        {
            var model = await _service.GetTransactionPreview(id);

            return View("Preview/_CheckCashing", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GenerateCheckPreview(int id)
        {
            byte[] pdf = await _service.GenerateCheckPreview(id);

            return File(
                pdf,
                "application/pdf",
                $"Check_{id}.pdf");
        }

        //REPORTE DE DOMESTIC
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DomesticPreviewPdf(int id)
        {
            var model = await _service.GetTransactionPreview(id);

            return View("Preview/_DomesticTransfer", model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GenerateDomesticPreview(int id)
        {
            byte[] pdf = await _service.GenerateDomesticPreview(id);

            return File(
                pdf,
                "application/pdf",
                $"Domestic_{id}.pdf");
        }

        //REPORTE DE MONEY ORDER
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> MoneyOrderPreviewPdf(int id)
        {
            var model = await _service.GetTransactionPreview(id);

            return View("Preview/_MoneyOrder", model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GenerateMoneyOrderPreview(int id)
        {
            byte[] pdf = await _service.GenerateMoneyOrderPreview(id);

            return File(
                pdf,
                "application/pdf",
                $"MoneyOrder_{id}.pdf");
        }

        //REPORTE DE PAID SERVICE
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PaidServicePreviewPdf(int id)
        {
            var model = await _service.GetTransactionPreview(id);

            return View("Preview/_PaidService", model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> GeneratePaidServicePreview(int id)
        {
            byte[] pdf = await _service.GeneratePaidServicePreview(id);

            return File(
                pdf,
                "application/pdf",
                $"PaidService_{id}.pdf");
        }

    }
}
