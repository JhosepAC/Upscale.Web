using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Upscale.Web.Models;

namespace Upscale.Web.Controllers
{
    /// <summary>
    /// Handles the main landing pages and global application states like errors
    /// Base authorization is required for all actions except where specified
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// Renders the main dashboard or landing page for authenticated users.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Renders the privacy policy page.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Global error handling action. 
        /// Configured to prevent caching of sensitive error details.
        /// </summary>
        [AllowAnonymous] // Ensures the error page is accessible even if the user session expired
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}