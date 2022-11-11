using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Eshop.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private EshopContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductsController(EshopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment; 
        }
        [Area("Admin")]
        public async Task<IActionResult> Index(int p=1)
        {
            var eshopcontext = _context.Products.Include(p => p.ProductType);
           
            return View(await eshopcontext.ToListAsync());
        }
        [Area("Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Clear();
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var account = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        [Area("Admin")]
        public IActionResult Delete(int? id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            var product = _context.Products.FirstOrDefault(x => x.Id == id);
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        [Area("Admin")]
        public IActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name");
            return View();
        }
        [Area("Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,ImageFile,Status")] Product product)
        {
            if(ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                if (product.ImageFile != null)
                {
                    var fileName = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var uploadFolder = Path.Combine(_environment.WebRootPath, "img", "product");
                    var uploadPath = Path.Combine(uploadFolder, fileName);
                    using (FileStream fs = System.IO.File.Create(uploadPath))
                    {
                        product.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    product.Image = fileName;
                    _context.Products.Update(product);
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);

        }
        [Area("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null||_context.Products==null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }
        [Area("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SKU,Name,Description,Price,Stock,ProductTypeId,Image,ImageFile,Status")] Product product)
        {
            if(id != product.Id)
            {
                return NotFound();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    var filename = product.Id.ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var uploadFolder = Path.Combine(_environment.WebRootPath, "img", "product");
                    var uploadPath = Path.Combine(uploadFolder, filename);
                    using(FileStream fs = System.IO.File.Create(uploadPath))
                    {
                        product.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    product.Image = filename;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "Id", "Name", product.ProductTypeId);
            return View(product);
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
    
}
