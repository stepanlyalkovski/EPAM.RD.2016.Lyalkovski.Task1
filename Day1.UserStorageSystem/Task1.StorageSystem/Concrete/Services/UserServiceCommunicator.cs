namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Entities;
    using NetworkServiceCommunication;
    using NetworkServiceCommunication.Entities;

    [Serializable]
    public class UserServiceCommunicator : MarshalByRefObject, IDisposable
    {
        private Sender<User> sender;

        private Task recieverTask;

        private CancellationTokenSource tokenSource;

        private Receiver<User> receiver;

        public UserServiceCommunicator(Sender<User> sender, Receiver<User> receiver)
        {
            this.sender = sender;
            this.receiver = receiver;
        }

        public UserServiceCommunicator(Sender<User> sender) : this(sender, null)
        {            
        }

        public UserServiceCommunicator(Receiver<User> receiver) : this(null, receiver)
        {          
        }

        public event EventHandler<UserDataApdatedEventArgs> UserAdded;

        public event EventHandler<UserDataApdatedEventArgs> UserDeleted;

        public event EventHandler RepositoryClear;

        public async void RunReceiver()
        {
            await receiver.AcceptConnection();
            if (receiver == null)
            {
                return;
            }

            tokenSource = new CancellationTokenSource();
            recieverTask = Task.Run((Action)ReceiveMessages, tokenSource.Token);
        }

        public void ConnectGroup(IEnumerable<IPEndPoint> endPoints)
        {
            sender.ConnectGroup(endPoints);
        }
       
        public void StopReceiver()
        {
            if (tokenSource.Token.CanBeCanceled)
            {
                tokenSource.Cancel();
            }
        }

        public void SendAdd(UserDataApdatedEventArgs args)
        {
            if (sender == null)
            {
                return;
            }

            Send(new ServiceMessage<User>
            {
                Entity = args.User,
                MessageType = MessageType.Add
            });
        }

        public void SendClear(EventArgs args)
        {
            Send(new ServiceMessage<User>
            {
                MessageType = MessageType.Clear
            });
        }

        public void SendDelete(UserDataApdatedEventArgs args)
        {
            if (sender == null)
            {
                return;
            }

            Send(new ServiceMessage<User>
            {
                Entity = args.User,
                MessageType = MessageType.Delete
            });
        }

        public void Dispose()
        {
            receiver?.Dispose();
            sender?.Dispose();
        }

        protected virtual void OnUserDeleted(object sender, UserDataApdatedEventArgs args)
        {
            UserDeleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            UserAdded?.Invoke(sender, args);
        }

        protected virtual void OnRepositoryClear(object sender, UserDataApdatedEventArgs args)
        {
            RepositoryClear?.Invoke(sender, args);
        }

        private void Send(ServiceMessage<User> message)
        {
            sender.Send(message);
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                if (tokenSource.IsCancellationRequested)
                {
                    return;
                }

                var message = receiver.Receive();
                var args = new UserDataApdatedEventArgs
                {
                    User = message.Entity
                };
                switch (message.MessageType)
                {
                    case MessageType.Add:
                        OnUserAdded(this, args);
                        break;
                    case MessageType.Delete:
                        OnUserDeleted(this, args);
                        break;
                    case MessageType.Clear:
                        OnRepositoryClear(this, args);
                        break;
                }
            }
        }
    }
}