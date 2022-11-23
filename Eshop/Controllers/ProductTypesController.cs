using Eshop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Eshop.Models;

namespace Eshop.Controllers
{
	public class ProductTypesController : Controller
	{
		EshopContext context;

		public ProductTypesController(EshopContext eshop)
		{
			context = eshop;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> ProductswithProductType(int? Id)
		{
            ViewBag.admin = HttpContext.Session.GetString("admin");
            ViewBag.username = HttpContext.Session.GetString("username");
			HomeModel home = new HomeModel();
			

            if (Id.HasValue)
			{
				home.ListProducts =await context.Products.Include(i=> i.ProductType).Where(x => x.ProductTypeId == Id).ToListAsync();
				home.ListProductTypes = await context.ProductTypes.ToListAsync();
				return View(home);
            }
			else
			{
				home.ListProducts =await context.Products.Include(i => i.ProductType).ToListAsync();
				home.ListProductTypes = await context.ProductTypes.ToListAsync();
				return View(home);
			}
			
		}

}
}
