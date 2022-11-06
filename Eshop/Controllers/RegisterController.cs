using Eshop.Data;
using Eshop.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Eshop.Controllers
{
    public class RegisterController : Controller
    {
        private readonly EshopContext _context;

        public RegisterController(EshopContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Username,Password,Email,Phone,Address,FullName,IsAdmin,Avatar,Status")] Account _account)
        {
                var check = _context.Accounts.FirstOrDefault(s => s.Email == _account.Email && s.Username == _account.Username);
                if (check == null)
                {
                    //_account.Password = GetMD5(_account.Password);
                    _context.Accounts.Add(_account);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewBag.error = "Email/Tên đăng nhập đã tồn tại";
                    return View();
                }
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
                    return RedirectToAction("Index", "Home", new { area = "Admin", controller = "Home", action = "Index"});
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

    }

}
