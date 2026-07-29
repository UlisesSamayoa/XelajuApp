using Microsoft.AspNetCore.Mvc;
using TransferApp.Services;

namespace TransferApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleService _service;
        public RolesController(RoleService service)
        {
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _service.GetRoles();
            return View(roles);
        }
    }
}
