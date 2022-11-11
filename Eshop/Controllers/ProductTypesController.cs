using Eshop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
			

            if (Id.HasValue)
			{
				var lstProduct =await context.Products.Where(x => x.ProductTypeId == Id).ToListAsync();
                return View(lstProduct);
            }
			else
			{
				var lstProduct =await context.Products.ToListAsync();
                return View(lstProduct);
            }
			
		}

}
}
