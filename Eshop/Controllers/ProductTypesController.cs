using Eshop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
		public IActionResult ProductswithProductType(int? Id)
		{
			var lstProduct= context.Products.Where(x => x.ProductTypeId == Id).ToList();
			return View(lstProduct);
		}

}
}
