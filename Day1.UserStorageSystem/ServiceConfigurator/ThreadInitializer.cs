using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Entities;

namespace ServiceConfigurator
{
    public class ThreadInitializer
    {
        private static IEnumerable<User> userTestCollection = new List<User>
        {
            new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP123",
                BirthDate = DateTime.Now
            },
            new User
            {
                FirstName = "Petr",
                LastName = "Petrov",
                PersonalId = "MP456",
                BirthDate = DateTime.Now
            },
            new User
            {
                FirstName = "Bob",
                LastName = "Smith",
                PersonalId = "MP789",
                BirthDate = DateTime.Now
            },
            new User
            {
                FirstName = "Jack",
                LastName = "Jackson",
                PersonalId = "MP999",
                BirthDate = DateTime.Now
            }
        };

        private static readonly Func<User, bool> SearchFoAllUserPredicate = u => u.PersonalId != String.Empty;
        private static string Lines { get; } = String.Join("", Enumerable.Repeat("-", 30));

        public static IEnumerable<Thread> InitializeThreads(MasterUserService master, IEnumerable<SlaveUserService> slaves)
        {
            IList<Thread> threads = new List<Thread>();

            if (master != null)
            {
                var masterThread = new Thread(() =>
                {
                    User previousUser = null;
                    while (true)
                    {
                        foreach (var user in userTestCollection)
                        {
                            master.Add(user);
                            Thread.Sleep(6000);
                            if (previousUser != null)
                            {
                                master.Delete(previousUser);
                            }
                            previousUser = user;
                            Thread.Sleep(6000);
                            Console.WriteLine(Lines + "\n" + "Master Search: ");
                            var userIds = master.SearchForUsers(new[] { SearchFoAllUserPredicate });
                            Console.Write("User's IDs: ");
                            foreach (var userId in userIds)
                            {
                                Console.Write(userId + " ");
                            }
                            Console.WriteLine("\n" + Lines + "\n");
                        }
                    }
                });
                masterThread.IsBackground = true;
                masterThread.Start();
                threads.Add(masterThread);
            }

            foreach (var slave in slaves)
            {
                var slaveThread = new Thread(() =>
                {
                    while (true)
                    {
                        var userIds = slave.SearchForUsers(new[]
                        {
                            SearchFoAllUserPredicate
                        });
                        Console.WriteLine(Lines);
                        Console.Write(slave.Name + " User's IDs: ");
                        foreach (var user in userIds)
                        {
                            Console.Write(user + " ");
                        }
                        Console.WriteLine("\n" + Lines);
                        Thread.Sleep(2000);
                    }

                });
                slaveThread.IsBackground = true;
                slaveThread.Start();
                slaveThread.IsBackground = true;
                threads.Add(slaveThread);
            }
            return threads;
        }
    }
}