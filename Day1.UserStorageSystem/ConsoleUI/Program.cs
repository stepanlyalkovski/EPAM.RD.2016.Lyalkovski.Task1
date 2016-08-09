using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ServiceConfigurator;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Entities;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<UserService> services = UserServiceInitializer.InitializeServices().ToList();
            //Console.Clear();
            Console.WriteLine("=========== Welcome to Console App ===========");
            
            var master = services.FirstOrDefault(s => s is MasterUserService);
            
            if (master != null)
            {
                master.Initialize();
                AddSomeMasterThreads((MasterUserService)master);
            }

            var cmd = Console.ReadLine();
            if (cmd == "save")
            {
                master.Save();

            }
        }

        private static void AddSomeMasterThreads(MasterUserService master)
        {
            Random rand = new Random();
            ThreadStart masterSearch = () =>
            {

                while (true)
                {
                    var serachresult = master.SearchForUsers(new Func<User, bool>[]
                    {
                        u => u.PersonalId != null
                    });
                    Console.Write("Another master thread search result: ");
                    foreach (var result in serachresult)
                    {
                        Console.Write(result + " ");
                    }
                    Console.WriteLine();
                    Thread.Sleep((int)(rand.NextDouble() * 5000));
                }

            };
            ThreadStart masterAdd = () =>
            {
                var uniqueUser = new User
                {
                    PersonalId = "Uniquie12345",
                    LastName = "Smith",
                    FirstName = "Bob",
                    BirthDate = DateTime.Now

                };
                while (true)
                {
                    master.Add(uniqueUser);
                    Thread.Sleep((int)(rand.NextDouble() * 8000));
                    master.Delete(uniqueUser);
                    Thread.Sleep((int)(rand.NextDouble() * 5000));
                }
            };
            Thread masterSearchThread = new Thread(masterSearch);
            Thread masterSearchThread2 = new Thread(masterSearch);
            Thread masterAddThread = new Thread(masterAdd) { IsBackground = true };
            masterSearchThread.IsBackground = true;
            masterSearchThread2.IsBackground = true;
            masterAddThread.Start();
            masterSearchThread.Start();
            masterSearchThread2.Start();

        }
    }
}
