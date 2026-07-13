using Microsoft.AspNetCore.Mvc;

namespace TransferApp.Models
{
    public class ReportsModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
