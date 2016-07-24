using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using NetworkServiceCommunication.Entities; 
namespace NetworkServiceCommunication
{
    [Serializable]
    public class Receiver<TEntity> : IDisposable
    {
        private Socket listener;
        private Socket reciever;
        public IPEndPoint IpEndPoint { get; set; }
        public Receiver(IPAddress ipAddress, int port)
        {
            IpEndPoint = new IPEndPoint(ipAddress, port);
            listener = new Socket(AddressFamily.InterNetwork,
               SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(IpEndPoint);
            listener.Listen(1);
        }

        public Task AcceptConnection()
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Wait Connection");
                reciever = listener.Accept();
                Console.WriteLine("Connection accepted");
            });

        }

        public ServiceMessage<TEntity> Receive()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            ServiceMessage<TEntity> message;

            using (var networkStream = new NetworkStream(reciever, false))
            {

                message = (ServiceMessage<TEntity>)formatter.Deserialize(networkStream);
            }
            Console.WriteLine("Message received!");
            return message;
        }

        //public void RunReceiver()
        //{
        //    var message = Receive();
        //    switch (message.MessageType)
        //    {
        //        case MessageType.Add: OnUserAdded(this, message); break;
        //        case MessageType.Delete: OnUserDeleted(this, message); break;

        //    }
                
        //}

        public void Dispose()
        {
            reciever.Close();
            listener.Close();
        }
    }
}