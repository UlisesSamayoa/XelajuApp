using Microsoft.AspNetCore.Mvc;
using TransferApp.Security;
using TransferApp.Services;
using TransferApp.ViewModels;

namespace TransferApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleService _service;
        public RolesController(RoleService service)
        {
            _service = service;
        }

        [Permission("Roles.View")]
        public async Task<IActionResult> Index()
        {
            var roles = await _service.GetRoles();
            return View(roles);
        }

        [HttpGet]
        [Permission("Roles.Create")]
        public async Task<IActionResult> Permissions(int id)
        {
            var role = await _service.GetRoleById(id);
            if (role == null)
                return NotFound();
            var availablePermissions = await _service.GetPermissions();
            var rolePermissions = await _service.GetRolePermissions(id);
            var model = new RoleViewModel
            {
                IdRole = role.IdRole,
                Name = role.Name,
                Description = role.Description,
                AvailablePermissions = availablePermissions,
                SelectedPermissions = rolePermissions.Select(x => x.IdPermission).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Permission("Roles.Create")]
        public async Task<IActionResult> Permissions(RoleViewModel model)
        {
            var result = await _service.SaveRolePermissions(model.IdRole, model.SelectedPermissions);
            if (result.Result != 1)
            {
                ModelState.AddModelError("", result.Message);
                model.AvailablePermissions = await _service.GetPermissions();
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
