using System;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using NetworkServiceCommunication;
using NetworkServiceCommunication.Entities;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Concrete.Services
{
    public class UserServiceCommunicator
    {
        public event EventHandler<UserDataApdatedEventArgs> UserAdded;
        public event EventHandler<UserDataApdatedEventArgs> UserDeleted;
        private Sender<User> _sender;
        private Task recieverTask;
        private CancellationTokenSource tokeSource;
        private Receiver<User> _receiver;

        public void RunReceiver()
        {
            var tokenSource = new CancellationTokenSource();
            recieverTask = Task.Run((Action)ReceiveMessages, tokeSource.Token);
        }

        public void StopReceiver()
        {
            if (tokeSource.Token.CanBeCanceled)
            {
                tokeSource.Cancel();
            }
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                var message = _receiver.Receive();
                var args = new UserDataApdatedEventArgs
                {
                    User = message.Entity
                };
                switch (message.MessageType)
                {
                    case MessageType.Add: OnUserAdded(this, args); break;
                    case MessageType.Delete: OnUserDeleted(this, args); break;
                }
            }
        }

        public void SendAdd(UserDataApdatedEventArgs args)
        {
            Send(new ServiceMessage<User>
            {
                Entity = args.User,
                MessageType = MessageType.Add
            });
        }
        public void SendDelete(UserDataApdatedEventArgs args)
        {
            Send(new ServiceMessage<User>
            {
                Entity = args.User,
                MessageType = MessageType.Delete
            });
        }

        private void Send(ServiceMessage<User> message)
        {
            _sender.Send(message);
        }

        protected virtual void OnUserDeleted(object sender, UserDataApdatedEventArgs args)
        {
            UserDeleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            UserAdded?.Invoke(sender, args);
        }
    }
}