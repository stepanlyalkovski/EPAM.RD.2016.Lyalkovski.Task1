using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using Task1.StorageSystem.Concrete.Services;

namespace ServiceConfigurator
{
    public class WcfServiceInitializer
    {
        /// <summary>
        /// Create wcf service
        /// </summary>
        /// <param name="service">UserService that will be created into wcf service</param>
        /// <returns>created wcf service host</returns>
        public static ServiceHost CreateWcfService(UserService service)
        {
            string localAddress = "127.0.0.1"; // GetLocalIpAddress();
            Uri serviceUri = new Uri($"http://{localAddress}:8080/UserService/" + service.Name);
            ServiceHost host = new ServiceHost(service, serviceUri);
            host.Open();

            foreach (Uri uri in host.BaseAddresses)
            {
                Console.WriteLine("\t{0}", uri.ToString());
            }

            Console.WriteLine();
            Console.WriteLine("Number of dispatchers listening : {0}", host.ChannelDispatchers.Count);
            foreach (System.ServiceModel.Dispatcher.ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                Console.WriteLine("\t{0}, {1}", dispatcher.Listener.Uri.ToString(), dispatcher.BindingName);
            }

            Console.WriteLine();

            return host;
        }
    }
}