using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceConfigurator;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Entities;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<UserService> services = ServiceInitializer.InitializeServices().ToList();
            string cmd = String.Empty;
            int requiredNumber = 0;
            while (cmd != "exit")
            {
                ServiceHelper.PrintServiceList(services);
                Console.WriteLine("Enter word 'service' and than type number'(service 1)");
                cmd = Console.ReadLine();
                var words = cmd.Split();
                bool parsed = false;
                if (words.Length > 1)
                {
                    parsed = Int32.TryParse(words.Skip(1).First(), out requiredNumber);
                }
                if (parsed)
                {
                    //here must be some awesome code
                }
            }
           
        }
    }
}
