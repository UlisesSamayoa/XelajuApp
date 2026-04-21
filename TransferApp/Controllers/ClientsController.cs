using Microsoft.AspNetCore.Mvc;

namespace TransferApp.Controllers
{
    public class ClientsController : Controller
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
