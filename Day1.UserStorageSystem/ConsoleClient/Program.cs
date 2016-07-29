using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleClient.ServiceReference1;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var master = new UserServiceContractClient("master");
            //var slave = new UserServiceContractClient("slave_1");
            User user = new User
            {
                FirstNamek__BackingField = "first_Name",
                LastNamek__BackingField = "Last_Name",
                BirthDatek__BackingField = DateTime.Now,
                PersonalIdk__BackingField = "MP1987"
            };
            
            Console.WriteLine("ready");
            Console.ReadLine();
            master.Add(user);
            Console.ReadLine();
            master.Delete(user);
            Console.ReadLine();
            //slave.Add(user);
        }
    }
}
