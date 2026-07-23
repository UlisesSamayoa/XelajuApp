using Microsoft.AspNetCore.Mvc;
using TransferApp.Security;
using TransferApp.Services;
using TransferApp.ViewModels;

namespace TransferApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _service;
        private readonly PasswordService _password;

        public UsersController(UserService service, PasswordService password)
        {
            _service = service;
            _password = password;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _service.GetUsers();

            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _service.SaveUser(model);
            if (result.Result != 1)
            {
                ModelState.AddModelError("", result.Message);

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            var user = await _service.GetUserById(id);
            return Json(user);
        }

        public IActionResult TestPassword()
        {
            string password = "Admin123!";
            string hash = _password.HashPassword(password);
            bool ok = _password.VerifyPassword(hash, password);
            return Content($"""Hash:{hash}Valid: {ok}""");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _service.GetUserById(id);
            if (user == null)
                return NotFound();
            var model = new UserViewModel
            {
                IdUser = user.IdUser,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                MustChangePassword = user.MustChangePassword
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _service.SaveUser(model);
            if (result.Result != 1)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
