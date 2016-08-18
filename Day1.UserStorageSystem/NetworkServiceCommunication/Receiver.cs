using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using NetworkServiceCommunication.Entities; 
namespace NetworkServiceCommunication
{
    [Serializable]
    public class Receiver<TEntity> : IDisposable
    {
        private Socket listener;
        private Socket reciever;
        protected TraceSource TraceSource = new TraceSource("StorageSystem");
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
                Debug.WriteLine("Wait Connection");
                reciever = listener.Accept();
                Debug.WriteLine("Connection accepted");
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

            Debug.WriteLine("Message received!");

            //this.reciever.Send(Encoding.UTF8.GetBytes("RECEIVED"));

            return message;
        }
        public void Dispose()
        {
            reciever?.Close();
            listener?.Close();
        }
    }
}