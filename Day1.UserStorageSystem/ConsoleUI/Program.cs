using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceConfigurator;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;

namespace ConsoleUI
{
    class Program
    {
        //public static User SimpleUser { get; set; } = new User
        //{
        //    FirstName = "Ivan2",
        //    LastName = "Ivanov2",
        //    PersonalId = "MP12345",
        //    BirthDate = DateTime.Now,
        //};

        //public static User AnotherUser { get; set; } = new User
        //{
        //    FirstName = "Bob",
        //    LastName = "Smith",
        //    PersonalId = "MP9999",
        //    BirthDate = DateTime.Now,
        //};
        //public static User AnotherUser2 { get; set; } = new User
        //{
        //    FirstName = "Jack",
        //    LastName = "Smith",
        //    PersonalId = "MP3423",
        //    BirthDate = DateTime.Now,
        //};

        static void Main(string[] args)
        {
            IList<UserService> services = UserServiceInitializer.InitializeServices().ToList();
            //Console.Clear();
            Console.WriteLine("=========== Welcome to Console App ===========");
            //master.Initialize();
            var master = services.FirstOrDefault(s => s is MasterUserService);

            //if (master != null)
            //{
            //    AddSomeMasterThreads((MasterUserService)master);
            //}
            string cmd = String.Empty;
            int requiredNumber = 0;
            while (cmd != "exit")
            {
                ServiceHelper.PrintServiceList(services);
                Console.WriteLine("Enter word 'service' and than type number'(service 1)");
                cmd = Console.ReadLine();
                var words = cmd.Split();
                bool parsed = false;

                if (cmd == "exit")
                {

                    return;
                    //master.Add(AnotherUser);
                }
                if (cmd == "stop")
                {
                    var slave = services.First(s => s is SlaveUserService);
                    slave.Communicator.StopReceiver();
                }

               
                //if (words.Length > 1)
                //{
                //    parsed = Int32.TryParse(words.Skip(1).First(), out requiredNumber);
                //}
                //if (parsed)
                //{
                //    //here must be some awesome code
                //}
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
                    Thread.Sleep((int)(rand.NextDouble() * 5000));
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
