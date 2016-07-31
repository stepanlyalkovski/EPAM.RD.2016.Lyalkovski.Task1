using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using Task1.StorageSystem.Concrete.Services;

namespace ServiceConfigurator
{
    public class WcfServiceInitializer
    {
        public static ServiceHost CreateWcfService(UserService service)
        {
            string localAddress = GetLocalIpAddress();
            Uri serviceUri = new Uri($"http://{localAddress}:8080/UserService/" + service.Name);
            ServiceHost host = new ServiceHost(service, serviceUri);
            host.Open();

            #region Output dispatchers listening
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
            #endregion

            return host;
        }

        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}