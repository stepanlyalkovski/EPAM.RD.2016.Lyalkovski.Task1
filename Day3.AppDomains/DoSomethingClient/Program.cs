using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using MyInterfaces;
//using MyLibrary;

namespace DoSomethingClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var input = new Input
            {
                Users = new User[]
                {
                    new User
                    {
                        Id = 1,
                        Name = "Vasily",
                        Age = 23
                    },
                    new User
                    {
                        Id = 2,
                        Name = "Semen",
                        Age = 35
                    },
                    new User
                    {
                        Id = 3,
                        Name = "Pawel",
                        Age = 22
                    }
                }
            };

            Method1(input);
            Method2(input);
        }

        private static void Method1(Input input)
        {
            // Create a domain with name MyDomain.
            AppDomain domain = AppDomain.CreateDomain("MyDomain");
            
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"MyDomain\MyLibrary.dll");
            var loader = (DomainAssemblyLoader)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(DomainAssemblyLoader).FullName);

            try
            {
                Result result = loader.LoadFile(path, input); // TODO: Use loader here.

                Console.WriteLine("Method1: {0}", result.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            // Unload domain
            AppDomain.Unload(domain);
        }

        private static void Method2(Input input)
        {
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyDomain")
            };

            // TODO: Create a domain with name MyDomain and setup from appDomainSetup.
            AppDomain domain = AppDomain.CreateDomain("MyDomain", null, appDomainSetup);

            var loader = (DomainAssemblyLoader)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(DomainAssemblyLoader).FullName);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"MyDomain\MyLibrary.dll");
            try
            {
                Result result = loader.LoadFrom(path, input); // TODO: Use loader here.

                Console.WriteLine("Method2: {0}", result.Value);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
            }

            AppDomain.Unload(domain);
        }
    }
}
