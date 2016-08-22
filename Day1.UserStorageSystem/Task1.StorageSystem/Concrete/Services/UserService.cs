namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using Entities;
    using Interfaces;
    using Interfaces.Repository;
    using SearchCriteries.UserCriteries;
    using Validation;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    [ServiceKnownType(typeof(ICriteria<User>))]
    [ServiceKnownType(typeof(CriterionMales))]
    public abstract class UserService : MarshalByRefObject, IUserServiceContract
    {
        protected UserService(
            INumGenerator numGenerator,
            ValidatorBase<User> validator,
            IRepository<User> repository) : this(numGenerator, validator, repository, false)
        {      
        }

        protected UserService(
                       INumGenerator numGenerator,
                       ValidatorBase<User> validator,
                       IRepository<User> repository,
                       bool loggingEnabled)
        {
            this.NumGenerator = numGenerator;
            Validator = validator;
            this.Repository = repository;
            TraceSource = new TraceSource("StorageSystem");

            this.LoggingEnabled = loggingEnabled;
            Debug.WriteLine($"Initializing UserService:\nDomain: {AppDomain.CurrentDomain.FriendlyName}");
        }

        public ICriteria<User> Criteria { get; set; } = new CriterionFemales();

        public ValidatorBase<User> Validator { get; set; }

        public UserServiceCommunicator Communicator { get; set; }

        public int LastGeneratedId { get; protected set; }

        public string Name { get; set; }

        protected INumGenerator NumGenerator { get; set; }

        protected IRepository<User> Repository { get; set; }

        protected TraceSource TraceSource { get; set; }

        protected bool LoggingEnabled { get; set; }

        protected ReaderWriterLockSlim StorageLock { get; set; } = new ReaderWriterLockSlim();

        public int Add(User user)
        {
            StorageLock.EnterWriteLock();
            try
            {
                if (LoggingEnabled)
                {
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"Adding User: {user.LastName} {user.PersonalId}");
                }

                int id = AddStrategy(user);
                if (LoggingEnabled)
                {
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"User was added: {user.LastName} {user.PersonalId}. Id = {id}");
                }

                return id;
            }
            finally
            {
                StorageLock.ExitWriteLock();
            }            
        }

        public void Delete(User user)
        {
            StorageLock.EnterWriteLock();
            try
            {
                if (LoggingEnabled)
                {
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"Deleting User: {user.LastName} {user.PersonalId}");
                }

                DeleteStrategy(user);

                if (LoggingEnabled)
                {
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"User {user.LastName} {user.PersonalId} was deleted");
                }
            }
            finally
            {
                StorageLock.ExitWriteLock();
            }
        }

        public virtual List<int> SearchForUsers(Func<User, bool>[] predicates)
        {
            StorageLock.EnterReadLock();
            try
            {
                return Repository.SearhByPredicate(predicates).ToList();
            }
            finally
            {
                StorageLock.ExitReadLock();
            }          
        }

        public virtual List<int> SearchForUsers(ICriteria<User>[] criteries)
        {
            StorageLock.EnterReadLock();
            try
            {
                return Repository.SearchByCriteria(criteries).ToList();
            }
            finally
            {
                StorageLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Add communicator to service to communicate through a network
        /// </summary>
        /// <param name="communicator">initialized communicator with receiver or sender</param>
        public virtual void AddCommunicator(UserServiceCommunicator communicator)
        {
            if (communicator == null)
            {
                return;
            }

            Communicator = communicator;
        }

        /// <summary>
        /// Allow services to specify additional functionality to save method 
        /// </summary>
        public abstract void Save();

        public abstract void Initialize(); // get collection from xml file and get last generated Id

        /// <summary>
        /// Allow services to specify additional functionality to add method 
        /// </summary>
        /// <param name="user">service user</param>
        /// <returns>Id of added user</returns>
        protected abstract int AddStrategy(User user);

        /// <summary>
        /// Allow services to specify additional functionality to delete method 
        /// </summary>
        /// <param name="user">service user</param>
        protected abstract void DeleteStrategy(User user);
    }
}
