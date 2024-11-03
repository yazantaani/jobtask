using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Task_DotNet_Mvc.Data;
using Task_DotNet_Mvc.Models;

namespace Task_DotNet_Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to find the user by username
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Username);

                if (user != null)
                {
                    // Verify the password
                    if (user.PasswordHash == HashPassword(model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, false);
                        return RedirectToAction("UserList", "User");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid username or password.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }

        private string HashPassword(string password)
        {
            // Simple password hashing example (you may want to use a more secure method in production)
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return System.BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

    }
}