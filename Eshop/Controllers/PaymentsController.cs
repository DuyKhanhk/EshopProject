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
        public IActionResult Index()
        {
            var name = HttpContext.Session.GetString("username");
            ViewBag.username = name;
            var acountid = _context.Accounts.SingleOrDefault(i => i.Username == name);
            var eshopContext = _context.Invoices.Where(i => i.AccountId == acountid.Id).OrderByDescending(i => i.IssuedDate);

            if (eshopContext == null)
            {
                return RedirectToAction("NotFound_OK", "Home");
            }
            return View(eshopContext.ToList());
        }

        public async Task<IActionResult> InvoiceDetails(int? id)
        {
            ViewBag.username = HttpContext.Session.GetString("username");
            var invDetails = _context.InvoiceDetails.Include(i => i.Invoice)
                                                   .Include(i => i.Product)
                                                   .Where(i => i.InvoiceId == id);
            return View(await invDetails.ToListAsync());
        }

        // GET: Payments/Create
        public IActionResult Payment(int Total)
        {
            var name = HttpContext.Session.GetString("username");
            Random rnd = new Random();
            int num;
            string num2;
            do
            {
                num = rnd.Next();
                num2 = num.ToString();
            } while (_context.Invoices.SingleOrDefault(i => i.Code == num2) != null);
            var account = _context.Accounts.SingleOrDefault(i => i.Username == name);

            ViewBag.username = name;
            ViewBag.total = Total;
            ViewBag.phone = account.Phone;
            ViewBag.address = account.Address;
            ViewBag.number = num;
            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment([Bind("Code,ShippingAddress,ShippingPhone,Total")] Invoice invoices)
        {
            var name = HttpContext.Session.GetString("username");
            var account = _context.Accounts.SingleOrDefault(i => i.Username == name);
            if (ModelState.IsValid)
            {
                DateTime date = DateTime.Now;

                Invoice invoice = new Invoice()
                {
                    Code = invoices.Code.ToString(),
                    AccountId = account.Id,
                    IssuedDate = date,
                    ShippingAddress = invoices.ShippingAddress,
                    ShippingPhone = invoices.ShippingPhone,
                    Total = invoices.Total,
                    Status = true

                };
                _context.Add(invoice);
                await _context.SaveChangesAsync();

                var inv = _context.Invoices.SingleOrDefault(u => u.Code == invoices.Code.ToString());//Lấy ra invoice vừa mới tạo

                var lstInvDetails = HttpContext.Session.Get<List<Cart>>("GioHang");
                List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();
                foreach (var item in lstInvDetails)
                {
                    InvoiceDetail invoiceDetail = new InvoiceDetail()
                    {
                        InvoiceId = inv.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Quantity * item.Product.Price
                    };

                    var productStock = _context.Products.SingleOrDefault(i => i.Id == item.ProductId);
                    productStock.Stock -= item.Quantity;

                    _context.Products.Update(productStock);
                    await _context.SaveChangesAsync();


                    invoiceDetails.Add(invoiceDetail);
                }

                _context.InvoiceDetails.AddRange(invoiceDetails);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("GioHang");

                ViewBag.username = name;
                return RedirectToAction(nameof(Index));
            }


            Random rnd = new Random();
            int num;
            string num2;
            do
            {
                num = rnd.Next();
                num2 = num.ToString();
            } while (_context.Invoices.SingleOrDefault(i => i.Code == num2) != null);

            ViewBag.total = invoices.Total;
            ViewBag.number = num;
            ViewBag.username = name;

            ViewBag.ErrorMsg = "Thanh toán thất bại";

            return View();
        }

    }
}
