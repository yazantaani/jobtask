using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Task_DotNet_Mvc.Data;
using Task_DotNet_Mvc.Models;

namespace Task_DotNet_Mvc.Services
{
    public class UserImportService
    {

        private readonly ApplicationDbContext _context;
        private Timer _timer;

        public UserImportService(ApplicationDbContext context)
        {
            _context = context;
            _timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(10)); // Check every 10 seconds
        }

        private void ProcessQueue(object state)
        {
            const int batchSize = 1000;

            List<User> usersToInsert = new List<User>();
            while (UserImportQueue.Users.TryDequeue(out User user))
            {
                usersToInsert.Add(user);
                if (usersToInsert.Count == batchSize)
                {
                    InsertUsers(usersToInsert);
                    usersToInsert.Clear();
                }
            }

            if (usersToInsert.Count > 0)
            {
                InsertUsers(usersToInsert); // Insert remaining users
            }
        }

        private void InsertUsers(List<User> users)
        {
            try
            {
                _context.Users.AddRange(users);
                _context.SaveChanges(); // Save changes in one go
            }
            catch (Exception ex)
            {
                // Log or handle the error
                Console.WriteLine($"Error inserting users: {ex.Message}");
            }
        }
    }

    public static class UserImportQueue
    {
        public static ConcurrentQueue<User> Users = new ConcurrentQueue<User>();

    }
}