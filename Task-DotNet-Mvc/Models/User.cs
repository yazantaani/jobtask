using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_DotNet_Mvc.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string PasswordHash { get; set; }
    }


}