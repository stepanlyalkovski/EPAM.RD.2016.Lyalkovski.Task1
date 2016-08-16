using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
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

                Console.WriteLine("----- " + service.Endpoint.Address + " -----" + service.Endpoint.Name);
                Console.WriteLine("cmd: add, delete, search, save, init, exit");
                Console.WriteLine("(init - clear repository and take users from xml file)");
                Console.WriteLine("(save - save users to xml file), add/delete - one default user");
                var readLine = Console.ReadLine();
                if (readLine == null) continue;

                string input = readLine.ToLower();
                try
                {
                    switch (input)
                    {
                        case "add": service.Add(SimpleUser); break;
                        case "delete":
                            service.Delete(SimpleUser);
                            break;
                        case "search":
                            {
                                CriterionFemales criteria = new CriterionFemales();
                                var users = service.SearchForUsers(new[] { new CriterionPersonalId() });
                                Console.WriteLine("Serch result: ");
                                foreach (var user in users)
                                {
                                    Console.Write(user + " ");
                                }
                                Console.WriteLine();
                                Console.ReadLine();


                            }
                            break;
                        case "save":
                        {
                            service.Save();
                        }
                            break;;
                        case "init":
                        {
                            service.Initialize();
                        }
                            break;
                        case "exit":
                            inProcess = false; break;
                        default: continue;
                    }
                }
                catch (FaultException excp)
                {
                    Console.WriteLine("You're not allowed to invoke this operation. Inner Details: ");
                    Console.WriteLine(excp.Message);
                    Console.ReadLine();
                }

            }

        }
    }
}