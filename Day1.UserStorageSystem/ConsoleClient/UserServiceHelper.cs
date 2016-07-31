using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using ConsoleClient.ServiceReference1;

namespace ConsoleClient
{
    public class UserServiceHelper
    {
        private static readonly User SimpleUser = new User
        {
            FirstNamek__BackingField = "Ivan",
            LastNamek__BackingField = "Ivanov",
            BirthDatek__BackingField = DateTime.Now,
            PersonalIdk__BackingField = "MP777"
        };
        public static IEnumerable<UserServiceContractClient> InitializeServices()
        {
            var names = GetServiceNames();

            return names.Select(name => new UserServiceContractClient(name)).ToList();
        }

        private static IEnumerable<string> GetServiceNames()
        {
            var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var sectionGroup = ServiceModelSectionGroup.GetSectionGroup(cfg);

            var endpoints = sectionGroup?.Client.Endpoints;
            return endpoints?.OfType<ChannelEndpointElement>().Select(endPoint => endPoint.Name).ToList();
        }

        public static void GetServiceMenu(UserServiceContractClient service)
        {
            bool inProcess = true;
            while (inProcess)
            {
                Console.Clear();
                Console.WriteLine("----- " + service.Endpoint.Address + " -----");
                Console.WriteLine("cmd: add, delete, search, exit");
                var readLine = Console.ReadLine();
                if (readLine == null) continue;

                string input = readLine.ToLower();

                switch (input)
                {
                    case "add": service.Add(SimpleUser); break;
                    case "delete": service.Delete(SimpleUser);
                        break;
                    case "exit":
                        inProcess = false; break;
                    default: continue;
                }
            }

        }
    }
}