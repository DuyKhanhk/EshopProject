using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Eshop.Helpers;


namespace Eshop.Controllers
{
	public class CartsController : Controller
	{
		EshopContext context;

		public CartsController(EshopContext eshopContext)
		{
			context=eshopContext;
		}

        public List<Cart> Carts
        {
            get
            {
                var data = HttpContext.Session.Get<List<Cart>>("GioHang");
                if (data == null)
                {
                    data = new List<Cart>();
                }
                return data;
            }
        }

        public IActionResult Index()
		{
           
            ViewBag.username = HttpContext.Session.GetString("username");
            return View(Carts);
		}

        public IActionResult AddToCart(int id, int quantity, string type="Normal")
		{
           
            var name = HttpContext.Session.GetString("username");
			var product = context.Products.SingleOrDefault(p => p.Id == id);
			var myCart = Carts;
			var item = myCart.SingleOrDefault(p => p.ProductId == id);
			if(item == null)//chua co
			{
				
				var user = context.Accounts.SingleOrDefault(p=> p.Username == name);
				item = new Cart()
				{
					
					Account = user,
					AccountId = user.Id,
					ProductId = id,
					Product=product,
					Quantity = quantity,

				};
				myCart.Add(item);
			}
			else
			{
				item.Quantity += quantity;
                if (item.Quantity > item.Product.Stock)
                {
                    item.Quantity = item.Product.Stock;
                }
            }
			HttpContext.Session.Set("GioHang", myCart);

			if (type == "ajax")
			{
				return Json(new
				{
					quantity = context.Carts.Sum(u => u.Quantity)
				});
				
            }
			var total = context.Carts.Sum(u => u.Quantity * u.Product.Price)+18000;

            HttpContext.Session.SetInt32("total", total);
			return RedirectToAction("Index");
		}

		public IActionResult Remove(int? id)
		{
			var cart = Carts;
			if (id != null)
			{
				cart.RemoveAll(u => u.ProductId == id);
				HttpContext.Session.Set("GioHang", cart);
			}
			else { HttpContext.Session.Remove("GioHang"); 
			}
			return RedirectToAction("Index");

			
        }
	}
}
