using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Eshop.Controllers
{
    public class HomeController : Controller
    { 
        private readonly EshopContext _context;

        public HomeController(EshopContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.admin = HttpContext.Session.GetString("admin");
            ViewBag.username = HttpContext.Session.GetString("username");
            HomeModel home = new HomeModel();
            home.ListProducts =await _context.Products.Include(p => p.ProductType).ToListAsync();
            home.ListProductTypes =await _context.ProductTypes.ToListAsync();
            return View(home);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotFound_OK()
        {
            return View();
        }
    }
}