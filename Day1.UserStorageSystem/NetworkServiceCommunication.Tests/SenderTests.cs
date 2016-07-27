using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkServiceCommunication.Entities;
using NUnit.Framework;
using Task1.StorageSystem.Entities;

namespace NetworkServiceCommunication.Tests
{
    [TestFixture]
    public class SenderTests
    {
        [Test]
        public void Test()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress receiverAddress = null;
            int receiverPort1 = 2020;
            int receiverPort2 = 2021;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    receiverAddress = ip;

                }
            }

            var receiver = new Receiver<User>(receiverAddress, receiverPort1);
            var receiver2 = new Receiver<User>(receiverAddress, receiverPort2);
            var sender = new Sender<User>();
            var task1 = StartReceiver(receiver);
            var task2 = StartReceiver(receiver2);
            var point1 = new IPEndPoint(receiverAddress, receiverPort1);
            var point2 = new IPEndPoint(receiverAddress, receiverPort2);
            sender.ConnectGroup(new List<IPEndPoint> {point1, point2});
            Console.WriteLine("Connected!");
            Thread.Sleep(3000);
            sender.Send(new ServiceMessage<User>
            {
                Entity = new User
                {
                    LastName = "LastNameFromSender"
                },
                MessageType = MessageType.Add
            });
            sender.Dispose();
            receiver.Dispose();
        }

        private Task StartReceiver(Receiver<User> receiver)
        {
            return Task.Run(() =>
            {
                Console.WriteLine("Wait for connection");
                receiver.AcceptConnection();
                Console.WriteLine("Sender connected to receiver");
                var message = receiver.Receive();
                Console.WriteLine(message.Entity.LastName);
            });
        }
    }
}

