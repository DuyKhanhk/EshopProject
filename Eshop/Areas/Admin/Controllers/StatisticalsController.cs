using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatisticalsController : Controller
    {
        EshopContext _context;
        public StatisticalsController(EshopContext context)
        {
           _context = context;
        }

        public IActionResult ListInvOfWeek()
        {
            DateTime weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek+1);

            var latesInvoice = _context.Invoices.Include(i=> i.Account).Where(i=>i.IssuedDate>=weekStart).OrderByDescending(i=>i.IssuedDate);


            var temp = latesInvoice.ToList().Count();
            HttpContext.Session.SetInt32("countOder", temp);

            if (latesInvoice != null)
            return View(latesInvoice);

            return NotFound();
        }

        public IActionResult SpendMoney()
        {
            var Money = _context.Invoices.Include(i => i.Account).GroupBy(i => i.Account.FullName).Select(a=> new Invoice{ Name = a.Key,Total = a.Sum(b=> b.Total)}).ToList();
                
            if(Money!=null)
            return View(Money);

            return NotFound();
        }

        public IActionResult TopProduct(int top)
        {
            ViewBag.Top = top;
            var topProduct = _context.InvoiceDetails.Include(p => p.Product)
                                                    .GroupBy(i => i.Product.Name)
                                                    .Select(a => new InvoiceDetail { ProductName = a.Key, Quantity = a.Sum(c => c.Quantity) })
                                                    .OrderByDescending(i => i.Quantity)
                                                    .ToList().Take(top);
            if(topProduct!=null)
            return View(topProduct);

            return NotFound();
        }

        public IActionResult Sales(int today)
        {
            DateTime date = DateTime.Today;
            switch (today)
            {
                case 0:
                    {
                        var sales = _context.Invoices.Where(o=> o.IssuedDate.Month==date.Month).GroupBy(i => i.IssuedDate.Day).Select(a => new Invoice { Id = a.Key, Total = a.Sum(b => b.Total) }).ToList();
                        ViewBag.sales = sales;
                        if (sales != null)
                            return View(sales);
                    }
                    break;
                    case 1:
                    {
                        var sales = _context.Invoices.Where(o => o.IssuedDate.Year == date.Year).GroupBy(i => i.IssuedDate.Month).Select(a => new Invoice { Id = a.Key, Total = a.Sum(b => b.Total) }).ToList();
                        ViewBag.sales = sales;
                        if (sales != null)
                            return View(sales);
                    }
                    break;
                case 2:
                    {
                        var sales = _context.Invoices.GroupBy(i => i.IssuedDate.Year).Select(a => new Invoice {Id=a.Key ,Total = a.Sum(b => b.Total) }).ToList();
                        ViewBag.sales = sales;
                        if (sales != null)
                            return View(sales);
                    }
                    break;
                default: break;
                    
            }

            return NotFound();
        }

        public IActionResult Turnover(int today)
        {
            DateTime date = DateTime.Today;
            switch (today)
            {
                case 0:
                    {
                        var sales = _context.InvoiceDetails.Include(p=> p.Invoice)
                                                           .Include(p=> p.Product)
                                                           .Where(o => o.Invoice.IssuedDate.Month == date.Month)
                                                           .GroupBy(i => i.Invoice.IssuedDate.Day).Select(a => new Invoice { Id = a.Key, Total = a.Sum(b => b.UnitPrice-b.Product.Price*b.Quantity) })
                                                           .ToList();
                        ViewBag.sales = sales;
                        if (sales != null)
                            return View(sales);
                    }
                    break;
                case 1:
                    {
                        var sales = _context.InvoiceDetails.Include(p => p.Invoice)
                                                           .Include(p => p.Product)
                                                           .Where(o => o.Invoice.IssuedDate.Year == date.Year)
                                                           .GroupBy(i => i.Invoice.IssuedDate.Month).Select(a => new Invoice { Id = a.Key, Total = a.Sum(b => b.UnitPrice - b.Product.Price * b.Quantity) })
                                                           .ToList();
                        ViewBag.sales = sales;
                        if (sales != null)
                            return View(sales);
                    }
                    break;
                case 2:
                    {
                        var sales = _context.InvoiceDetails.Include(p => p.Invoice)
                                                          .Include(p => p.Product)
                                                          .GroupBy(i => i.Invoice.IssuedDate.Year).Select(a => new Invoice { Id = a.Key, Total = a.Sum(b => b.UnitPrice - b.Product.Price * b.Quantity) })
                                                          .ToList();
                        ViewBag.sales = sales;
                        if (sales != null)
                            return View(sales);
                    }
                    break;
                default: break;

            }

            return NotFound();
        }
    }
}
