using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Eshop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountsController : Controller
    {
        private readonly EshopContext _context;
        private readonly IWebHostEnvironment _environment;

        public AccountsController(EshopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = HttpContext.Request.Cookies["username"];
            return View(await _context.Accounts.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            HttpContext.Session.Clear();
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,ConfirmPassword,Email,Phone,Address,FullName,IsAdmin,Avatar,ImageFile,Status")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                if (account.ImageFile != null)
                {
                    var fileName = account.Username.ToString() + Path.GetExtension(account.ImageFile.FileName);
                    var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "avatar");
                    var uploadPath = Path.Combine(uploadFolder, fileName);
                    using (FileStream fs = System.IO.File.Create(uploadPath))
                    {
                        account.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    account.Avatar = fileName;
                    _context.Accounts.Update(account);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,ConfirmPassword,Email,Phone,Address,FullName,IsAdmin,Avatar,ImageFile,Status")] Account account)
        {
            if (id != account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var file = account.Username.ToString() + Path.GetExtension(account.ImageFile.FileName);
                    var uploadFolder = Path.Combine(_environment.WebRootPath, "images", "avatar");
                    var path = Path.Combine(uploadFolder, file);
                    using (FileStream fs = System.IO.File.Create(path))
                    {
                        
                        account.ImageFile.CopyTo(fs);
                        fs.Flush();
                    }
                    account.Avatar = file;
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.Id))
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
            return View(account);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var account = _context.Accounts.FirstOrDefault(x => x.Id == id);
            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
    }
}
