namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using NetworkServiceCommunication;
    using NetworkServiceCommunication.Entities;
    using Entities;

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

        public UserServiceCommunicator(Sender<User> sender) : this(sender, null) { }

        public UserServiceCommunicator(Receiver<User> receiver) : this(null, receiver) { }

        public event EventHandler<UserDataApdatedEventArgs> UserAdded;

        public event EventHandler<UserDataApdatedEventArgs> UserDeleted;
        public async void RunReceiver()
        {
            await this.receiver.AcceptConnection();
            if (this.receiver == null) return;
            this.tokenSource = new CancellationTokenSource();
            this.recieverTask = Task.Run((Action)this.ReceiveMessages, this.tokenSource.Token);
        }

        public void ConnectGroup(IEnumerable<IPEndPoint> endPoints)
        {
            this.sender.ConnectGroup(endPoints);
        }
       
        public void StopReceiver()
        {
            if (this.tokenSource.Token.CanBeCanceled)
            {
                this.tokenSource.Cancel();
            }
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                if (this.tokenSource.IsCancellationRequested) return;
                var message = this.receiver.Receive();
                var args = new UserDataApdatedEventArgs
                {
                    User = message.Entity
                };
                switch (message.MessageType)
                {
                    case MessageType.Add:
                        this.OnUserAdded(this, args);
                        break;
                    case MessageType.Delete:
                        this.OnUserDeleted(this, args);
                        break;
                }
            }
        }

        public void SendAdd(UserDataApdatedEventArgs args)
        {
            if (this.sender == null) return;

            this.Send(new ServiceMessage<User>
            {
                Entity = args.User,
                MessageType = MessageType.Add
            });
        }
        public void SendDelete(UserDataApdatedEventArgs args)
        {
            if (this.sender == null) return;

            this.Send(new ServiceMessage<User>
            {
                Entity = args.User,
                MessageType = MessageType.Delete
            });
        }

        protected virtual void OnUserDeleted(object sender, UserDataApdatedEventArgs args)
        {
            this.UserDeleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            this.UserAdded?.Invoke(sender, args);
        }

        public void Dispose()
        {
            this.receiver?.Dispose();
            this.sender?.Dispose();
        }
        private void Send(ServiceMessage<User> message)
        {
            this.sender.Send(message);
        }

    }
}