using Microsoft.AspNetCore.Mvc;

namespace TransferApp.Controllers
{
    public class TransactionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Update()
        {
            return View();
        }
    }
}
