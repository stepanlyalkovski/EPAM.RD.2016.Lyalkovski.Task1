using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.StorageSystem.Concrete.Services
{
    public abstract class UserService : MarshalByRefObject
    {
        protected INumGenerator NumGenerator;
        protected IRepository<User> Repository;                    
        protected TraceSource TraceSource;
        protected bool LoggingEnabled;
        protected ReaderWriterLockSlim storageLock = new ReaderWriterLockSlim();
        public ValidatorBase<User> Validator { get; set; }
        public UserServiceCommunicator Communicator { get; set; }
        public int LastGeneratedId { get; protected set; } //temp
        public string Name { get; set; }
        protected UserService(INumGenerator numGenerator, ValidatorBase<User> validator,
            IRepository<User> repository) : this(numGenerator, validator, repository, false)
        {

        }

        protected UserService(INumGenerator numGenerator, ValidatorBase<User> validator,
                                IRepository<User> repository, bool loggingEnabled)
        {
            NumGenerator = numGenerator;
            Validator = validator;
            Repository = repository;
            TraceSource = new TraceSource("StorageSystem");

            LoggingEnabled = loggingEnabled;
            Debug.WriteLine($"Initializing UserService:\nDomain: {AppDomain.CurrentDomain.FriendlyName}");
        }

        public int Add(User user)
        {
            storageLock.EnterWriteLock();
            try
            {
                if (LoggingEnabled)
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"Adding User: {user.LastName} {user.PersonalId}");

                return AddStrategy(user);
            }
            finally
            {
                storageLock.ExitWriteLock();
            }
            
        }

        public void Delete(User user)
        {
            storageLock.EnterWriteLock();
            try
            {
                if (LoggingEnabled)
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"Deleting User: {user.LastName} {user.PersonalId}");

                DeleteStrategy(user);
            }
            finally
            {
                storageLock.ExitWriteLock();
            }
            
        }

        protected abstract int AddStrategy(User user);
        protected abstract void DeleteStrategy(User user);

        public virtual List<int> SearchForUsers(Func<User, bool>[] predicates)
        {
            storageLock.EnterReadLock();
            try
            {
                return Repository.SearhByPredicate(predicates).ToList();
            }
            finally
            {
                storageLock.ExitReadLock();
            }
            
        }

        public virtual void AddCommunicator(UserServiceCommunicator communicator)
        {
            if (communicator == null) return;
            Communicator = communicator;
        }

        public abstract void Save();
        public abstract void Initialize(); // get collection from xml file and get last generated Id
    }
}
