namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Diagnostics;
    using Validation;
    using Entities;
    using Interfaces;
    using Interfaces.Repository;

    public class SlaveUserService : UserService
    {
        public SlaveUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository) 
            : this(numGenerator, validator, repository, false)
        { }
        public SlaveUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository, bool loggingEnabled) 
                        : base(numGenerator, validator, repository, loggingEnabled)
        { }
        protected override int AddStrategy(User user)
        {
            throw new NotSupportedException();
        }

        protected override void DeleteStrategy(User user)
        {
            throw new NotSupportedException();
        }

        public override void Save()
        {
            throw new NotSupportedException();
        }

        public override void Initialize()
        {
            throw new NotSupportedException();
        }

        private void OnAdded(object sender, UserDataApdatedEventArgs args)
        {
            this.StorageLock.EnterWriteLock();
            try
            {
                Debug.WriteLine("On Added! " + AppDomain.CurrentDomain.FriendlyName);
                this.Repository.Add(args.User);
                this.LastGeneratedId = args.User.Id;
            }
            finally
            {
                this.StorageLock.ExitWriteLock();
            }                
        }

        private void OnDeleted(object sender, UserDataApdatedEventArgs args)
        {
            this.StorageLock.EnterWriteLock();
            try
            {
                this.Repository.Delete(args.User);
            }
            finally
            {
                this.StorageLock.ExitWriteLock();
            }
            
        }
        public void Subscribe(MasterUserService master)
        {
            master.Deleted += this.OnDeleted;
            master.Added += this.OnAdded;
        }

        public override void AddCommunicator(UserServiceCommunicator communicator)
        {
            base.AddCommunicator(communicator);

            this.Communicator.UserAdded += this.OnAdded;
            this.Communicator.UserDeleted += this.OnDeleted;
        }
    }
}