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

        // ──────────────────────────────────────────────────────────
        //  PUBLICS
        // ──────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Welcome()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

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
                    return RedirectToAction("Locked", new { id = user.DocumentNumber });

                user.IsLocked = false;
                user.FailedAttempts = 0;
                user.LockoutEnd = null;
                await _context.SaveChangesAsync();
            }

            if (!VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.FailedAttempts++;

                if (user.FailedAttempts >= MaxFailedAttempts)
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

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.DocumentNumber)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Locked(string id)
        {
            ViewBag.DocumentNumber = id;
            return View();
        }

        // ──────────────────────────────────────────────────────────
        //  PROTECTEDS
        // ──────────────────────────────────────────────────────────

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var documentNumber = User.Identity?.Name;
            if (string.IsNullOrEmpty(documentNumber))
                return RedirectToAction("Login");

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.DocumentNumber == documentNumber);

            if (user == null) return RedirectToAction("Login");
            return View(MapToViewModel(user));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var editableFields = new[]
            {
                nameof(model.Email),
                nameof(model.SecondaryEmail),
                nameof(model.MobilePhone),
                nameof(model.SecondaryPhone)
            };

            foreach (var key in ModelState.Keys
                     .Where(k => !editableFields.Contains(k)))
                ModelState.Remove(key);

            var documentNumber = User.Identity?.Name;
            if (string.IsNullOrEmpty(documentNumber))
                return RedirectToAction("Login");

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.DocumentNumber == documentNumber);

            if (user == null) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                var fresh = MapToViewModel(user);
                model.FirstName = fresh.FirstName;
                model.FirstLastName = fresh.FirstLastName;
                model.SecondLastName = fresh.SecondLastName;
                model.DocumentType = fresh.DocumentType;
                model.DocumentNumber = fresh.DocumentNumber;
                model.BirthDate = fresh.BirthDate;
                model.Nationality = fresh.Nationality;
                model.Gender = fresh.Gender;
                model.JobTitle = fresh.JobTitle;
                model.Organization = fresh.Organization;
                model.ContractType = fresh.ContractType;
                model.HireDate = fresh.HireDate;
                model.IsActive = fresh.IsActive;
                return View(model);
            }

            user.Email = model.Email?.Trim() ?? user.Email;

            if (user.Profile != null)
            {
                user.Profile.SecondaryEmail = model.SecondaryEmail?.Trim();
                user.Profile.MobilePhone = model.MobilePhone?.Trim();
                user.Profile.SecondaryPhone = model.SecondaryPhone?.Trim();
            }

            await _context.SaveChangesAsync();

            TempData["ProfileSuccess"] = "Los datos han sido guardados correctamente.";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ExtendSession()
        {
            var token = Request.Headers["RequestVerificationToken"].ToString();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { error = "Token CSRF ausente." });

            try
            {
                var antiforgery = HttpContext.RequestServices
                    .GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
                await antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch
            {
                return Unauthorized(new { error = "Token CSRF inválido." });
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                HttpContext.User,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });

            return Ok(new { extended = true });
        }

        // ──────────────────────────────────────────────────────────
        //  HELPERS
        // ──────────────────────────────────────────────────────────

        private static ProfileViewModel MapToViewModel(Upscale.Web.Models.Entities.User user)
        {
            return new ProfileViewModel
            {
                FirstName = user.Profile?.FirstName ?? string.Empty,
                FirstLastName = user.Profile?.FirstLastName ?? string.Empty,
                SecondLastName = user.Profile?.SecondLastName ?? string.Empty,
                DocumentType = user.Profile?.DocumentType ?? string.Empty,
                DocumentNumber = user.DocumentNumber,
                BirthDate = user.Profile?.BirthDate ?? DateTime.MinValue,
                Nationality = user.Profile?.Nationality ?? string.Empty,
                Gender = user.Profile?.Gender ?? string.Empty,
                Email = user.Email ?? string.Empty,
                SecondaryEmail = user.Profile?.SecondaryEmail ?? string.Empty,
                MobilePhone = user.Profile?.MobilePhone ?? string.Empty,
                SecondaryPhone = user.Profile?.SecondaryPhone ?? string.Empty,
                JobTitle = user.Profile?.JobTitle ?? string.Empty,
                Organization = user.Profile?.Organization ?? string.Empty,
                ContractType = user.Profile?.ContractType ?? string.Empty,
                HireDate = user.Profile?.HireDate,
                IsActive = user.IsActive,
            };
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash == null || storedHash.Length != 64) return false;
            if (storedSalt == null || storedSalt.Length != 128) return false;

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
    }
}