using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
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
            Console.WriteLine("\n======== Service List ========");
            for (int i = 0; i < serviceCount; i++)
            {
                var service = userServices[i];
                Console.Write($"Service {i} : type = ");
                if (service is MasterUserService)
                {
                    Console.WriteLine(" Master");
                }
                else
                {
                    Console.WriteLine(" Slave");
                }

                Console.WriteLine("Current Domain: " + AppDomain.CurrentDomain.FriendlyName);
                Console.WriteLine("IsProxy: " + RemotingServices.IsTransparentProxy(service));
                var predicates = new Func<User, bool>[] { p => p.LastName != null };
                Console.Write("User's IDs: ");
                foreach (var user in service.SearchForUsers(predicates))
                {
                    Console.Write(user + " ");
                }

                Console.WriteLine("\n" + string.Concat(Enumerable.Repeat("-", 20)));
            }
        }
    }
}