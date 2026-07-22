using Microsoft.AspNetCore.Mvc;
using TransferApp.Services;

namespace TransferApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _service.GetUsers();

            return View(users);
        }
        public IActionResult Update()
        {
            return View();
        }
    }
}
