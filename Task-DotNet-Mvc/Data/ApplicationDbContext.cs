using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Task_DotNet_Mvc.Models;

namespace Task_DotNet_Mvc.Data
{
    public class ApplicationDbContext : DbContext
    {
            public ApplicationDbContext() : base("DefaultConnection") // "DefaultConnection" should match the connection string in Web.config
            {
            }

            public DbSet<User> Users { get; set; }

            // You can override OnModelCreating if you need custom configurations
            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Additional configurations, if needed
                // modelBuilder.Entity<User>().HasKey(u => u.Id); // Example: Setting a primary key
            }
        }
}