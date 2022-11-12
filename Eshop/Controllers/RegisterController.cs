using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Eshop.Controllers
{
    public class RegisterController : Controller
    {
        private readonly EshopContext _context;

        private readonly IWebHostEnvironment _environmemt;

        public RegisterController(EshopContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environmemt = environment;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id, Username, Password, ConfirmPassword, Email, Phone, Address, FullName, IsAdmin, Avatar, ImageFile, Status")] Account _account)
        {
            if (ModelState.IsValid)
            {
                
                var check = _context.Accounts.FirstOrDefault(s => s.Email == _account.Email || s.Username == _account.Username);
                if (check == null)
                {
                    //_account.Password = GetMD5(_account.Password);
                    var user = new Account()
                    {
                        Username = _account.Username,
                        Email = _account.Email,
                        Password = _account.Password,
                        Phone = _account.Phone,
                        Address = _account.Address,
                        FullName = _account.FullName,
                        IsAdmin = false,
                        Avatar = "customer.jpg",
                        ImageFile = _account.ImageFile,
                        Status = _account.Status,
                    };
                    _context.Accounts.Add(user);
                    await _context.SaveChangesAsync();

                    var imgAvtar = _context.Accounts.SingleOrDefault(i => i.Username == user.Username);
                    if (user.ImageFile != null)
                    {
                        var fileName = user.Username.ToString() + Path.GetExtension(user.ImageFile.FileName);
                        var uploadFolder = Path.Combine(_environmemt.WebRootPath, "images", "avatar");
                        var uploadPath = Path.Combine(uploadFolder, fileName);

                        using (FileStream fs = System.IO.File.Create(uploadPath))
                        {
                            user.ImageFile.CopyTo(fs);
                            fs.Flush();
                        }
                        user.Avatar = fileName;
                        _context.Accounts.Update(user);
                        _context.SaveChanges();
                    }

                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email/Tên đăng kí đã tồn tại";
                    return View();
                }
            }

            ViewBag.ErrorMsg = "Đăng kí thất bại";
            return View();
        }

        //create a string MD5
        //public static string GetMD5(string str)
        //{
        //    MD5 md5 = new MD5CryptoServiceProvider();
        //    byte[] fromData = Encoding.UTF8.GetBytes(str);
        //    byte[] targetData = md5.ComputeHash(fromData);
        //    string byte2String = null;

        //    for (int i = 0; i < targetData.Length; i++)
        //    {
        //        byte2String += targetData[i].ToString("x2");

        //    }
        //    return byte2String;
        //}

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("admin") == null|| HttpContext.Session.GetString("username") == null)
            { 
                return View(); 
            }
            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            //var f_password = GetMD5(Password);
            var account = _context.Accounts.FirstOrDefault(a => a.Username == Username && a.Password ==Password);
            if (account != null)
            {
                if (account.IsAdmin)
                { 
                    HttpContext.Session.SetString("admin", "Admin");
                    return Redirect("~/Admin/Home");
                }
                else
                {
                    HttpContext.Session.SetString("username", Username);
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.ErrorMsg = "Đăng nhập thất bại!!";
                return View();
            }
        }

        public IActionResult Logout()
        {
            if(HttpContext.Session.GetString("admin")=="Admin")
                HttpContext.Session.Remove("admin");
            else
                HttpContext.Session.Remove("username");

            return RedirectToAction("Index", "Home");
        }
        
        public async Task<IActionResult> Details()
        {
            if (HttpContext.Session.GetString("username") != null || HttpContext.Session.GetString("admin") != null)
            {
                string _name = HttpContext.Session.GetString("username");
                string _admin = HttpContext.Session.GetString("admin");
                var account = await _context.Accounts

                .FirstOrDefaultAsync(m => m.Username == _name||m.Username==_admin);
                if (account == null)
                {
                    return NotFound();
                }
                ViewBag.Fullname = account.FullName;
                ViewBag.admin = HttpContext.Session.GetString("admin");
                ViewBag.username = HttpContext.Session.GetString("username");
                return View(account);
            }
            else
            {
                return RedirectToAction("Login", "Register");
            }

        }

        // GET: Accounts/Edit/5
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
            ViewBag.admin = HttpContext.Session.GetString("admin");
            ViewBag.username = HttpContext.Session.GetString("username");
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Account _account)
        {
            if (id != _account.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (AccountExists(id))
                {
                    var user = new Account()
                    {
                        Id = id,
                        Username = _account.Username,
                        Email = _account.Email,
                        Password = _account.Password,
                        Phone = _account.Phone,
                        Address = _account.Address,
                        IsAdmin = false,
                        FullName = _account.FullName,
                        Avatar = "customer.jpg",
                    };

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details");
                }
                return NotFound();
                
            }
            return View(_account);
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }
        private bool FindAccountWnname(String name)
        {
            return _context.Accounts.Any(u => u.Username == name);
        }

    }

}
