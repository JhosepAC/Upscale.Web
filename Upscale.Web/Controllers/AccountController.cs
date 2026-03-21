using Microsoft.AspNetCore.Mvc;
using Upscale.Web.Data;
using Upscale.Web.Models.ViewModels;

namespace Upscale.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Welcome
        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            return View(model);
        }

        // GET: Account/Locked
        [HttpGet]
        public IActionResult Locked()
        {
            return View();
        }
    }
}