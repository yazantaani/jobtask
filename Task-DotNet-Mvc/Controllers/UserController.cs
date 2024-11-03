using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task_DotNet_Mvc.Data;
using Task_DotNet_Mvc.Models;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;
using OfficeOpenXml;
using Task_DotNet_Mvc.Services;

namespace Task_DotNet_Mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController()
        {
            _context = new ApplicationDbContext(); // Instantiate context directly if DI is not set up
        }

        public ActionResult UserList()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUser(User model)
        {
            if (ModelState.IsValid)
            {
                model.PasswordHash = HashPassword("GeneratedPassword"); // Generate and hash password
                _context.Users.Add(model);
                _context.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ImportExcel()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(file.InputStream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++) // Skip header row
                    {
                        var user = new User
                        {
                            Id = int.Parse(worksheet.Cells[row, 1].Text),
                            Name = worksheet.Cells[row, 2].Text,
                            Email = worksheet.Cells[row, 3].Text,
                            MobileNumber = worksheet.Cells[row, 4].Text,
                            PasswordHash = HashPassword(Guid.NewGuid().ToString()),
                            Photo = null
                        };
                        UserImportQueue.Users.Enqueue(user); // Enqueue user for processing
                    }
                }
            }
            return RedirectToAction("UserList");
        }


        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
