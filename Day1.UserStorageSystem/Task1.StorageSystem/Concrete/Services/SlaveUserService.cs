namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Diagnostics;
    using Entities;
    using Interfaces;
    using Interfaces.Repository;
    using Validation;

    public class SlaveUserService : UserService
    {
        public SlaveUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository)
            : this(numGenerator, validator, repository, false)
        {           
        }

        public SlaveUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository, bool loggingEnabled)
            : base(numGenerator, validator, repository, loggingEnabled)
        {          
        }
   
        public override void Save()
        {
            throw new NotSupportedException();
        }

        public override void Initialize()
        {
            throw new NotSupportedException();
        }

        public void Subscribe(MasterUserService master)
        {
            master.Deleted += OnDeleted;
            master.Added += OnAdded;
        }

        public override void AddCommunicator(UserServiceCommunicator communicator)
        {
            base.AddCommunicator(communicator);

            Communicator.UserAdded += OnAdded;
            Communicator.UserDeleted += OnDeleted;
            Communicator.RepositoryClear += OnRepositoryClear;
        }

        protected override int AddStrategy(User user)
        {
            throw new NotSupportedException();
        }

        protected override void DeleteStrategy(User user)
        {
            throw new NotSupportedException();
        }

        private void OnAdded(object sender, UserDataApdatedEventArgs args)
        {
            StorageLock.EnterWriteLock();
            try
            {
                Debug.WriteLine("On Added! " + AppDomain.CurrentDomain.FriendlyName);
                Repository.Add(args.User);
                LastGeneratedId = args.User.Id;
            }
            finally
            {
                StorageLock.ExitWriteLock();
            }
        }

        private void OnDeleted(object sender, UserDataApdatedEventArgs args)
        {
            StorageLock.EnterWriteLock();
            try
            {
                Repository.Delete(args.User);
            }
            finally
            {
                StorageLock.ExitWriteLock();
            }
        }

        private void OnRepositoryClear(object sender, EventArgs args)
        {
            Console.WriteLine("REPOSITORY CLEAR!");
            Repository.Clear();
        }
    }
}