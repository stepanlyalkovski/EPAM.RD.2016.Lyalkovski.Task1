using System;
using System.Collections.Generic;
using System.Linq;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Entities;

namespace ConsoleUI
{
    public class ServiceHelper
    {
        public static void PrintServiceList(IEnumerable<UserService> services)
        {
            var userServices = services as IList<UserService> ?? services.ToList();
            int serviceCount = userServices.Count();
            for (int i = 0; i < serviceCount; i++)
            {
                var service = userServices[i];
                Console.WriteLine($"Service {i} :");
                if (service is MasterUserService)
                    Console.WriteLine("Master");
                else
                {
                    Console.WriteLine("Slave");
                }
                Console.WriteLine("Current Domain: " + AppDomain.CurrentDomain.FriendlyName);
                var predicates = new Func<User, bool>[] { p => p.LastName != null };
                Console.WriteLine("Users: " + service.SearchForUsers(predicates));
                Console.WriteLine(string.Concat(Enumerable.Repeat("=", 30)));
            }
        }

    }
}