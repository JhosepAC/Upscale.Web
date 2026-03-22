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
using Upscale.Web.Services;

namespace Upscale.Web.Controllers
{
    /// <summary>
    /// Manages user authentication, session state, and profile information.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;

        public AccountController(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            // Prevent authenticated users from accessing the welcome page
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// Clears the user's authentication cookie and redirects to login.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Validates credentials and handles account lockout logic.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.DocumentNumber == model.DocumentNumber);

            // Generic error message to prevent username enumeration attacks
            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View(model);
            }

            if (user.IsLocked)
            {
                if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.Now)
                    return RedirectToAction("Locked", new { id = user.DocumentNumber });

                // Automatic unlock if lockout period has expired
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
                    user.LockoutEnd = DateTime.Now.AddMinutes(LockoutMinutes);
                    await _context.SaveChangesAsync();

                    // Fire-and-forget email notification
                    _ = SendAccountLockedEmailAsync(user);

                    return RedirectToAction("Locked", new { id = user.DocumentNumber });
                }

                await _context.SaveChangesAsync();
                ModelState.AddModelError(string.Empty, "Invalid credentials.");
                return View(model);
            }

            // Authentication success: reset security counters
            user.FailedAttempts = 0;
            user.IsLocked = false;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            var fullName = user.Profile != null
                ? $"{user.Profile.FirstName} {user.Profile.FirstLastName} {user.Profile.SecondLastName}"
                : user.DocumentNumber;

            var jobTitle = user.Profile?.JobTitle ?? "Sin cargo";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.DocumentNumber),
                new Claim("FullName", fullName),
                new Claim("JobTitle", jobTitle)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var documentNumber = User.Identity?.Name;
            if (string.IsNullOrEmpty(documentNumber)) return RedirectToAction("Login");

            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.DocumentNumber == documentNumber);

            if (user == null) return RedirectToAction("Login");

            return View(MapToViewModel(user));
        }

        /// <summary>
        /// Updates allowed profile fields while maintaining data integrity for read-only properties.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            // Define strictly editable fields to prevent over-posting attacks
            var editableFields = new[]
            {
                nameof(model.Email),
                nameof(model.SecondaryEmail),
                nameof(model.MobilePhone),
                nameof(model.SecondaryPhone)
            };

            // Remove validation errors for read-only fields
            foreach (var key in ModelState.Keys.Where(k => !editableFields.Contains(k)))
                ModelState.Remove(key);

            var documentNumber = User.Identity?.Name;
            var user = await _context.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.DocumentNumber == documentNumber);

            if (user == null) return RedirectToAction("Login");

            if (!ModelState.IsValid)
            {
                // Re-populate read-only data for the view
                var fresh = MapToViewModel(user);
                model.DocumentNumber = fresh.DocumentNumber;
                model.FirstName = fresh.FirstName;
                // ... populate other fields as needed for the UI
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

            TempData["ProfileSuccess"] = "Changes saved successfully.";
            return RedirectToAction(nameof(Profile));
        }

        /// <summary>
        /// AJAX endpoint to extend the authentication cookie lifetime based on user activity.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ExtendSession()
        {
            var token = Request.Headers["RequestVerificationToken"].ToString();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { error = "CSRF token missing." });

            try
            {
                var antiforgery = HttpContext.RequestServices.GetRequiredService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
                await antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch
            {
                return Unauthorized(new { error = "Invalid CSRF token." });
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

        private async Task SendAccountLockedEmailAsync(Upscale.Web.Models.Entities.User user)
        {
            try
            {
                var fullName = user.Profile != null
                    ? $"{user.Profile.FirstLastName} {user.Profile.SecondLastName}, {user.Profile.FirstName}".Trim()
                    : user.DocumentNumber;

                var htmlBody = AccountLockedEmailTemplate.Build(
                    fullName: fullName,
                    documentNumber: user.DocumentNumber,
                    lockoutMinutes: LockoutMinutes,
                    lockoutEnd: user.LockoutEnd ?? DateTime.Now.AddMinutes(LockoutMinutes));

                await _emailService.SendAsync(
                    toEmail: user.Email,
                    toName: fullName,
                    subject: "Account Security Alert: Temporary Lockout",
                    htmlBody: htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background email service failed for user {Id}", user.DocumentNumber);
            }
        }

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

        /// <summary>
        /// Compares the provided password against the stored salted hash using constant-time comparison.
        /// </summary>
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash?.Length != 64 || storedSalt?.Length != 128) return false;

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            // FixedTimeEquals prevents timing attacks
            return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
        }
    }
}