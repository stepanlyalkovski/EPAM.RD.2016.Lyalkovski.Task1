using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
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
            int receiverPort1 = 2050;
            int receiverPort2 = 2051;
            int receiverPort3 = 2052;
            int receiverPort4 = 2053;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    receiverAddress = ip;

                }
            }

            var receiver = new Receiver<User>(receiverAddress, receiverPort1);
            var receiver2 = new Receiver<User>(receiverAddress, receiverPort2);
            var receiver3 = new Receiver<User>(receiverAddress, receiverPort3);
            var receiver4 = new Receiver<User>(receiverAddress, receiverPort4);
            var sender = new Sender<User>();
            this.StartReceiver(receiver);
            this.StartReceiver(receiver2);
            StartReceiver(receiver3);
            StartReceiver(receiver4);
            var point1 = new IPEndPoint(receiverAddress, receiverPort1);
            var point2 = new IPEndPoint(receiverAddress, receiverPort2);
            var point3 = new IPEndPoint(receiverAddress, receiverPort3);
            var point4 = new IPEndPoint(receiverAddress, receiverPort4);
            sender.ConnectGroup(new List<IPEndPoint> {point1, point2, point3, point4});
            Console.WriteLine("Connected!");
            Thread.Sleep(3000);
            int iterations = 10;
            while (iterations-- > 0)
            {
                sender.Send(new ServiceMessage<User>
                {
                    Entity = new User
                    {
                        LastName = $"LastNameFromSender_{iterations}"
                    },
                    MessageType = MessageType.Add
                });
            }
            sender.Dispose();
            receiver.Dispose();
        }

        private async void StartReceiver(Receiver<User> receiver)
        {
            Console.WriteLine("Wait for connection");
            await receiver.AcceptConnection();
            while (true)
            {
                Console.WriteLine("Sender connected to receiver");
                var message = receiver.Receive();
                Console.WriteLine(message.Entity.LastName);
            }
        }

        [Test]
        public void SingleReceiverTest()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress receiverAddress = null;
            int receiverPort1 = 2055;
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    receiverAddress = ip;
                }
            }

            var receiver = new Receiver<User>(receiverAddress, receiverPort1);
            var sender = new Sender<User>();
            //Task tsk = receiver.AcceptConnection();
            this.StartReceiver(receiver);
            sender.Connect(receiver.IpEndPoint);
            Thread.Sleep(1000);
            int iterations = 20;
            int receive = iterations;
            ;
            while (iterations-- > 0)
            {
                sender.Send(new ServiceMessage<User>
                {
                    Entity = new User
                    {
                        LastName = $"LastNameFromSender_{iterations}"
                    },
                    MessageType = MessageType.Add
                });
            }

            //while (receive-- > 0)
            //{
            //    var message = receiver.Receive();
            //    Debug.WriteLine(message.Entity.LastName);
            //}
            Thread.Sleep(5000);
        }
    }
}

