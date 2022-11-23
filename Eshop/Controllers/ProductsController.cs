using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;

namespace Eshop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EshopContext _context;

        public ProductsController(EshopContext context)
        {
            _context = context;
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.admin = HttpContext.Session.GetString("admin");
            ViewBag.username = HttpContext.Session.GetString("username");
            return View(product);
        }

        public async Task<IActionResult> Search(string? key)
        {
            ViewBag.admin = HttpContext.Session.GetString("admin");
            ViewBag.username = HttpContext.Session.GetString("username");
           
            if (key == null || _context.Products==null) { return RedirectToAction("Index","Home"); }
            var lstProducts = await _context.Products.Include(p=> p.ProductType).Where(u => u.Name.Contains(key)).ToListAsync();
            if (lstProducts.Count()!=0)
            {
            return View(lstProducts);
            }
                return RedirectToAction("NotFound_OK","Home");
        }

        public async Task<IActionResult> SearchMultipleCriteria(int? Id)
        {
            ViewBag.admin = HttpContext.Session.GetString("admin");
            ViewBag.username = HttpContext.Session.GetString("username");
            HomeModel home = new HomeModel();


            if (Id.HasValue)
            {
                home.ListProducts = await _context.Products.Include(i => i.ProductType).Where(x => x.ProductTypeId == Id).ToListAsync();
                home.ListProductTypes = await _context.ProductTypes.ToListAsync();
                return View(home);
            }
            else
            {
                home.ListProducts = await _context.Products.Include(i => i.ProductType).ToListAsync();
                home.ListProductTypes = await _context.ProductTypes.ToListAsync();
                return View(home);
            }
        }
    }
}
