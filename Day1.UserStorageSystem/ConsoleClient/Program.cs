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
            var serviceList = UserServiceHelper.InitializeServices().ToList();
            bool inProcess = true;
            while (inProcess)
            {
                for (int i = 0; i < serviceList.Count; i++)
                {
                    Console.WriteLine(1 + i + ")" + serviceList[i].Endpoint.Address);
                }
                string input = Console.ReadLine();
                int number;
                bool parsed = Int32.TryParse(input, out number);

                if (parsed)
                {
                    UserServiceHelper.GetServiceMenu(serviceList[number - 1]);
                }
                else
                {
                    inProcess = false;
                }
            }
           
        }
    }
}
