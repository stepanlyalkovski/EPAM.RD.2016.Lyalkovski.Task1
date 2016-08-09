using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using NetworkServiceCommunication.Entities;

namespace NetworkServiceCommunication
{
    [Serializable]
    public class Sender<TEntity> : IDisposable
    {
        private IList<Socket> sockets = new List<Socket>();

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
                //TODO Make xml formatter
                BinaryFormatter formatter = new BinaryFormatter();
                using (NetworkStream networkStream = new NetworkStream(socket, false))
                {
                    formatter.Serialize(networkStream, message);
                }
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