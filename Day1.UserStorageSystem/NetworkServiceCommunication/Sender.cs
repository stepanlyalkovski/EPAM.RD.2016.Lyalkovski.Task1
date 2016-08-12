using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NetworkServiceCommunication.Entities;

namespace NetworkServiceCommunication
{
    [Serializable]
    public class Sender<TEntity> : IDisposable
    {
        private IList<Socket> sockets = new List<Socket>();
        protected TraceSource TraceSource = new TraceSource("StorageSystem");
        public void ConnectGroup(IEnumerable<IPEndPoint> ipEndPoints)
        {
            foreach (var ipEndPoint in ipEndPoints)
            {
                var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipEndPoint);
                this.sockets.Add(socket);
            }

        }
        public void Connect(IPEndPoint ipEndPoint)
        {
            var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);
            this.sockets.Add(socket);
        }

        public void Send(ServiceMessage<TEntity> message)
        {
            foreach (var socket in this.sockets)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (NetworkStream networkStream = new NetworkStream(socket, false))
                {
                    formatter.Serialize(networkStream, message);
                }

                //var byteMessage = new byte[1024];
                //int bytes = socket.Receive(byteMessage);
                //var resultMessage = Encoding.UTF8.GetString(byteMessage.Take(bytes).ToArray());
                //this.TraceSource.TraceEvent(TraceEventType.Critical, 10, resultMessage + " " + AppDomain.CurrentDomain.FriendlyName);
                //Debug.WriteLine(resultMessage);
            }

        }

        public void Dispose()
        {
            foreach (var socket in this.sockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}