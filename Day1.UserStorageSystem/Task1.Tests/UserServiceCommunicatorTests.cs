using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NetworkServiceCommunication;
using NUnit.Framework;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Entities;

namespace Task1.Tests
{
    [TestFixture]
    public class UserServiceCommunicatorTests
    {
        [Test]
        public void SendAddMessage_ServiceCommunicatorWithSenderAndTwoCommunicatorsWithReceivers_ConnectionEstablishedAndMessageReceivedByTwoCommunicators()
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

            Sender<User> sender = new Sender<User>();
            Receiver<User> slaveReceiver1 = new Receiver<User>(receiverAddress, receiverPort1);
            Receiver<User> slaveReceiver2 = new Receiver<User>(receiverAddress, receiverPort2);
            sender.Connect(new List<IPEndPoint> {slaveReceiver1.IpEndPoint, slaveReceiver2.IpEndPoint});
            slaveReceiver1.AcceptConnection();
            slaveReceiver2.AcceptConnection();
            UserServiceCommunicator masterCommunicator = new UserServiceCommunicator(sender);
            UserServiceCommunicator slaveCommunicator1 = new UserServiceCommunicator(slaveReceiver1);
            UserServiceCommunicator slaveCommunicator2 = new UserServiceCommunicator(slaveReceiver2);
            slaveCommunicator1.RunReceiver();
            slaveCommunicator2.RunReceiver();
            slaveCommunicator1.UserAdded += (o, args) => Console.WriteLine("Event Generated in Slave 1! " + args.User.LastName);
            slaveCommunicator2.UserAdded += (o, args) => Console.WriteLine("Event Generated in Slave 2! " + args.User.LastName);
            masterCommunicator.SendAdd(new UserDataApdatedEventArgs {User =  new User {LastName = "Smith"} });
            Thread.Sleep(2000);

            slaveCommunicator2.StopReceiver();
            slaveCommunicator1.StopReceiver();
            masterCommunicator.Dispose();
            slaveCommunicator2.Dispose();
            slaveCommunicator1.Dispose();

        }
    }
}