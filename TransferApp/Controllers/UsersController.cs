using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using TransferApp.Security;
using TransferApp.Services;
using TransferApp.ViewModels;

namespace TransferApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UsersController : Controller
    {
        private readonly UserService _service;
        private readonly RoleService _roleService;
        private readonly PasswordService _password;

        public UsersController(UserService service, PasswordService password, RoleService roleService)
        {
            _service = service;
            _password = password;
            _roleService = roleService;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _service.GetUsers();

            return View(users);
        }
        public async Task<IActionResult> Create()
        {
            var model = new UserViewModel
            {
                AvailableRoles = await _roleService.GetRoles()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _roleService.GetRoles();
                return View(model);
            }
            var result = await _service.SaveUser(model);
            if (result.Result != 1)
            {
                ModelState.AddModelError("", result.Message);
                model.AvailableRoles = await _roleService.GetRoles();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        //public async Task<IActionResult> Update(int id)
        //{
        //    var user = await _service.GetUserById(id);
        //    return Json(user);
        //}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _service.GetUserById(id);
            var roles = await _roleService.GetRoles();
            var userRoles = await _roleService.GetUserRoles(id);
            if (user == null)
                return NotFound();
            //var model = new UserViewModel
            //{
            //    IdUser = user.IdUser,
            //    Username = user.Username,
            //    FirstName = user.FirstName,
            //    LastName = user.LastName,
            //    Email = user.Email,
            //    IsActive = user.IsActive,
            //    MustChangePassword = user.MustChangePassword,
            //    AvailableRoles = roles,
            //    SelectedRoles = userRoles.Select(r => r.IdRole).ToList()
            //};
            var model = new UserEditViewModel
            {
                IdUser = user.IdUser,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                MustChangePassword = user.MustChangePassword,
                AvailableRoles = roles,
                SelectedRoles = userRoles.Select(r => r.IdRole).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableRoles = await _roleService.GetRoles();
                return View(model);
            }
            var result = await _service.SaveUser(model);
            if (result.Result != 1)
            {
                ModelState.AddModelError("", result.Message);
                model.AvailableRoles = await _roleService.GetRoles();
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
