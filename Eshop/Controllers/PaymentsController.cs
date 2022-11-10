using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eshop.Data;
using Eshop.Models;
using System.Xml.Linq;
using Eshop.Helpers;

namespace Eshop.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly EshopContext _context;

        public PaymentsController(EshopContext context)
        {
            _context = context;
        }

        
        // GET: Payments
        public async Task<IActionResult> Index()
        {
            ViewBag.username = HttpContext.Session.GetString("username");
            var eshopContext = _context.Invoices.Include(i => i.Account).OrderByDescending(i=>i.IssuedDate);
            return View(await eshopContext.ToListAsync());
        }

        public async Task<IActionResult> InvoiceDetails(int? id)
        {
            ViewBag.username = HttpContext.Session.GetString("username");
            var invDetails=_context.InvoiceDetails.Include(i=> i.Invoice)
                                                   .Include(i=>i.Product)
                                                   .Where(i => i.InvoiceId == id);
            return View(await invDetails.ToListAsync());
        }
        
        // GET: Payments/Create
        public IActionResult Payment(int Total)
        {
            var name= HttpContext.Session.GetString("username");
            Random rnd = new Random();
            int num;
            string num2;
            do
            {
                num = rnd.Next();
                num2 = num.ToString();
            } while (_context.Invoices.SingleOrDefault(i => i.Code == num2) != null);
            var account=_context.Accounts.SingleOrDefault(i=> i.Username==name);

            ViewBag.total = Total;
            ViewBag.phone=account.Phone;
            ViewBag.address = account.Address;
            ViewBag.number = num;
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(int Code,string ShippingAddress, string ShippingPhone, int Total)
        {
            var name= HttpContext.Session.GetString("username");
            if (ModelState.IsValid)
            {
                DateTime date = DateTime.Now;
                var account = _context.Accounts.SingleOrDefault(i => i.Username == name);
                Invoice invoice = new Invoice()
                {
                    Code = Code.ToString(),
                    AccountId = account.Id,
                    IssuedDate = date,
                    ShippingAddress = ShippingAddress,
                    ShippingPhone = ShippingPhone,
                    Total = Total,
                    Status = true

                };
                _context.Add(invoice);
                await _context.SaveChangesAsync();

                var inv = _context.Invoices.SingleOrDefault(u => u.Code == Code.ToString());//Lấy ra invoice vừa mới tạo

                var lstInvDetails = HttpContext.Session.Get<List<Cart>>("GioHang");
                List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();
                foreach (var item in lstInvDetails)
                {
                    InvoiceDetail invoiceDetail = new InvoiceDetail()
                    {
                        InvoiceId=inv.Id,
                        ProductId=item.ProductId,
                        Quantity=item.Quantity,
                        UnitPrice=item.Quantity*item.Product.Price
                    };
                    invoiceDetails.Add(invoiceDetail);
                }
                _context.InvoiceDetails.AddRange(invoiceDetails);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction("Index","Home");
        }

    }
}
