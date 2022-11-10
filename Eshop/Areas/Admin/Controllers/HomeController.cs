using Microsoft.AspNetCore.Mvc;

namespace Eshop.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("admin") != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Register", new { area = "" });
        }

    }
}
