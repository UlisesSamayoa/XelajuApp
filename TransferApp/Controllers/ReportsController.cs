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

    }
}
