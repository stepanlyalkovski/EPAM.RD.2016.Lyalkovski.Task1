using System;
using System.Diagnostics;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.StorageSystem.Concrete.Services
{
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
            storageLock.EnterWriteLock();
            try
            {
                Debug.WriteLine("On Added! " + AppDomain.CurrentDomain.FriendlyName);
                Repository.Add(args.User);
                LastGeneratedId = args.User.Id;
            }
            finally
            {
                storageLock.ExitWriteLock();
            }
                
        }

        private void OnDeleted(object sender, UserDataApdatedEventArgs args)
        {
            storageLock.EnterWriteLock();
            try
            {
                Repository.Delete(args.User);
            }
            finally
            {
                storageLock.ExitWriteLock();
            }
            
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
        }
    }
}