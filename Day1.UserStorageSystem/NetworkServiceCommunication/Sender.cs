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

        protected TraceSource TraceSource { get; set; } = new TraceSource("StorageSystem");

        /// <summary>
        /// Establish connection between master and required addresses
        /// </summary>
        /// <param name="ipEndPoints">end points that master will connect to</param>
        public void ConnectGroup(IEnumerable<IPEndPoint> ipEndPoints)
        {
            foreach (var ipEndPoint in ipEndPoints)
            {
                var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipEndPoint);
                sockets.Add(socket);
            }
        }

        public void Connect(IPEndPoint ipEndPoint)
        {
            var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipEndPoint);
            sockets.Add(socket);
        }

        /// <summary>
        /// send message to connected group of slaves
        /// </summary>
        /// <param name="message">message type and data</param>
        public void Send(ServiceMessage<TEntity> message)
        {
            foreach (var socket in sockets)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (NetworkStream networkStream = new NetworkStream(socket, false))
                {
                    formatter.Serialize(networkStream, message);
                }
            }
        }

        public void Dispose()
        {
            foreach (var socket in sockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}