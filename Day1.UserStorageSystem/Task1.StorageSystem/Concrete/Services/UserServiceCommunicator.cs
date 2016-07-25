using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using NetworkServiceCommunication;
using NetworkServiceCommunication.Entities;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Concrete.Services
{
    [Serializable]
    public class UserServiceCommunicator : MarshalByRefObject,IDisposable
    {
        public event EventHandler<UserDataApdatedEventArgs> UserAdded;
        public event EventHandler<UserDataApdatedEventArgs> UserDeleted;
        private Sender<User> _sender;
        private Task recieverTask;
        private CancellationTokenSource tokenSource;
        private Receiver<User> _receiver;

        public UserServiceCommunicator(Sender<User> sender, Receiver<User> receiver)
        {
            _sender = sender;
            _receiver = receiver;
        }

        public UserServiceCommunicator(Sender<User> sender) : this(sender, null) { }
        public UserServiceCommunicator(Receiver<User> receiver) : this(null, receiver) { }

        public async void RunReceiver()
        {
            await _receiver.AcceptConnection();
            if (_receiver == null) return;
            tokenSource = new CancellationTokenSource();
            recieverTask = Task.Run((Action)ReceiveMessages, tokenSource.Token);
        }

        public void Connect(IEnumerable<IPEndPoint> endPoints)
        {
            _sender.Connect(endPoints);
        }

        public void StopReceiver()
        {
            if (tokenSource.Token.CanBeCanceled)
            {
                tokenSource.Cancel();
            }
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                if(tokenSource.IsCancellationRequested) return;
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
            if (_sender == null) return;

            Send(new ServiceMessage<User>
            {
                Entity = args.User,
                MessageType = MessageType.Add
            });
        }
        public void SendDelete(UserDataApdatedEventArgs args)
        {
            if (_sender == null) return;

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

        public void Dispose()
        {
            _receiver?.Dispose();
            _sender?.Dispose();
        }
    }
}