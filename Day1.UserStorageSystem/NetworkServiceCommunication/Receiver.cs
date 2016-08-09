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
            this.IpEndPoint = new IPEndPoint(ipAddress, port);
            this.listener = new Socket(AddressFamily.InterNetwork,
               SocketType.Stream, ProtocolType.Tcp);
            this.listener.Bind(this.IpEndPoint);
            this.listener.Listen(1);
        }

        public Task AcceptConnection()
        {
            return Task.Run(() =>
            {
                Debug.WriteLine("Wait Connection");
                this.reciever = this.listener.Accept();
                Debug.WriteLine("Connection accepted");
            });

        }

        public ServiceMessage<TEntity> Receive()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            ServiceMessage<TEntity> message;

            using (var networkStream = new NetworkStream(this.reciever, false))
            {

                message = (ServiceMessage<TEntity>)formatter.Deserialize(networkStream);
            }
            Debug.WriteLine("Message received!");
            return message;
        }
        public void Dispose()
        {
            this.reciever?.Close();
            this.listener?.Close();
        }
    }
}