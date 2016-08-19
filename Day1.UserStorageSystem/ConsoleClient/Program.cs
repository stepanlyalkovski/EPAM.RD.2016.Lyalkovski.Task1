using System;
using System.Linq;

namespace ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var serviceList = UserServiceHelper.InitializeServices().ToList();
            bool inProcess = true;
            Console.WriteLine("=== DEMO client. Only for testing purposes ===\nPress any key...");
            Console.ReadLine();
            Console.Clear();
            while (inProcess)
            {
                for (int i = 0; i < serviceList.Count; i++)
                {                  
                    Console.WriteLine(1 + i + ")" + serviceList[i].Endpoint.Address);
                }

                string input = Console.ReadLine();
                int number;
                bool parsed = int.TryParse(input, out number);

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
