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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}