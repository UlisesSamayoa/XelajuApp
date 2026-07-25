using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TransferApp.Models;
using TransferApp.Services;
using TransferApp.ViewModels;

namespace TransferApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _service;
        public AuthController(UserService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _service.Authenticate(model);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            if (result.MustChangePassword)
            {
                return RedirectToAction(nameof(ChangePassword), new
                {
                    id = result.User!.IdUser
                });
            }
            await SignInUser(result.User!);
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
                new Claim("IdUser", user.IdUser.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var user = await _service.GetUserById(id);

            if (user == null || !user.MustChangePassword)
                return RedirectToAction(nameof(Login));
            return View(new ChangePasswordViewModel
            {
                IdUser = id
            });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await _service.ChangePassword(model.IdUser, model.NewPassword);
            if (result.Result != 1)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
            var user = await _service.GetUserById(model.IdUser);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }
            await SignInUser(user!);
            return RedirectToAction("Index", "Home");
            //return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
