using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Upscale.Web.Data;
using Upscale.Web.Models.ViewModels;

namespace Upscale.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int MaxFailedAttempts = 5;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Welcome() => View();

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.DocumentNumber == model.DocumentNumber);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Credenciales incorrectas.");
                return View(model);
            }

            if (user.IsLocked)
            {
                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now)
                {
                    return RedirectToAction("Locked", new { id = user.DocumentNumber });
                }
                else
                {
                    user.IsLocked = false;
                    user.FailedAttempts = 0;
                    user.LockoutEnd = null;
                    await _context.SaveChangesAsync();
                }
            }

            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.FailedAttempts++;

                if (user.FailedAttempts >= 5)
                {
                    user.IsLocked = true;
                    user.LockoutEnd = DateTime.Now.AddMinutes(15);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Locked", new { id = user.DocumentNumber });
                }

                await _context.SaveChangesAsync();
                ModelState.AddModelError(string.Empty, "Contraseña incorrecta.");
                return View(model);
            }

            user.FailedAttempts = 0;
            user.IsLocked = false;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.DocumentNumber) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            Console.WriteLine("Mensaje exitoso");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Locked(string id)
        {
            ViewBag.DocumentNumber = id;
            return View();
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash == null || storedHash.Length != 64)
                return false;

            if (storedSalt == null || storedSalt.Length != 128)
                return false;

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
            }
        }

    }
}