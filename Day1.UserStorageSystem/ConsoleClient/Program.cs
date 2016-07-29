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
            UserServiceContractClient client = new UserServiceContractClient();
            
            User user = new User
            {
                FirstNamek__BackingField = "first_Name",
                LastNamek__BackingField = "Last_Name",
                BirthDatek__BackingField = DateTime.Now,
                PersonalIdk__BackingField = "MP1987"
            };
            Console.WriteLine("ready");
            Console.ReadLine();
            client.Add(user);
            Console.ReadLine();
            client.Delete(user);
        }
    }
}
