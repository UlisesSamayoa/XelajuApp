using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            var claims = new List<Claim>
            {
                new Claim("IdUser", result.User!.IdUser.ToString()),
                new Claim(ClaimTypes.Name, result.User.Username),
                new Claim("FirstName", result.User.FirstName),
                new Claim("LastName", result.User.LastName),
                new Claim(ClaimTypes.NameIdentifier, result.User.IdUser.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
