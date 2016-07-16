using System;
using System.Collections.Generic;
using System.Linq;
using Task1.StorageSystem.Concrete.Services;

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
                Console.WriteLine(string.Concat(Enumerable.Repeat("=", 30)));
            }
        }

    }
}