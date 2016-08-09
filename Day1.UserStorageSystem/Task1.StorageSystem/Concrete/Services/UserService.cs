namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading;
    using SearchCriteries.UserCriteries;
    using Validation;
    using Entities;
    using Interfaces;
    using Interfaces.Repository;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    [ServiceKnownType(typeof(ICriteria<User>))]
    [ServiceKnownType(typeof(CriterionMales))]
    public abstract class UserService : MarshalByRefObject, IUserServiceContract
    {
        protected INumGenerator NumGenerator;
        protected IRepository<User> Repository;                    
        protected TraceSource TraceSource;
        protected bool LoggingEnabled;
        protected ReaderWriterLockSlim StorageLock = new ReaderWriterLockSlim();

        public ICriteria<User> Criteria = new CriterionFemales(); 

        public ValidatorBase<User> Validator { get; set; }

        public UserServiceCommunicator Communicator { get; set; }
        public int LastGeneratedId { get; protected set; }
        public string Name { get; set; }
        protected UserService(
                        INumGenerator numGenerator, 
                        ValidatorBase<User> validator,
                        IRepository<User> repository) : this(numGenerator, validator, repository, false)
        { }

        protected UserService(
                       INumGenerator numGenerator, 
                       ValidatorBase<User> validator,
                       IRepository<User> repository, 
                       bool loggingEnabled)
        {
            this.NumGenerator = numGenerator;
            this.Validator = validator;
            this.Repository = repository;
            this.TraceSource = new TraceSource("StorageSystem");

            this.LoggingEnabled = loggingEnabled;
            Debug.WriteLine($"Initializing UserService:\nDomain: {AppDomain.CurrentDomain.FriendlyName}");
        }

        public int Add(User user)
        {
            this.StorageLock.EnterWriteLock();
            try
            {
                if (this.LoggingEnabled)
                    this.TraceSource.TraceEvent(TraceEventType.Information, 0, $"Adding User: {user.LastName} {user.PersonalId}");

                int id = this.AddStrategy(user);

                if (this.LoggingEnabled)
                    this.TraceSource.TraceEvent(TraceEventType.Information, 0, $"User was added: {user.LastName} {user.PersonalId}. Id = {id}");
                return id;
            }
            finally
            {
                this.StorageLock.ExitWriteLock();
            }
            
        }

        public void Delete(User user)
        {
            this.StorageLock.EnterWriteLock();
            try
            {
                if (this.LoggingEnabled)
                    this.TraceSource.TraceEvent(TraceEventType.Information, 0, $"Deleting User: {user.LastName} {user.PersonalId}");

                this.DeleteStrategy(user);

                if (this.LoggingEnabled)
                    this.TraceSource.TraceEvent(TraceEventType.Information, 0, $"User {user.LastName} {user.PersonalId} was deleted");
            }
            finally
            {
                this.StorageLock.ExitWriteLock();
            }
            
        }

        protected abstract int AddStrategy(User user);
        protected abstract void DeleteStrategy(User user);

        public virtual List<int> SearchForUsers(Func<User, bool>[] predicates)
        {
            this.StorageLock.EnterReadLock();
            try
            {
                return this.Repository.SearhByPredicate(predicates).ToList();
            }
            finally
            {
                this.StorageLock.ExitReadLock();
            }
            
        }

        public virtual List<int> SearchForUsers(ICriteria<User>[] criteries)
        {
            this.StorageLock.EnterReadLock();
            try
            {
                return this.Repository.SearchByCriteria(criteries).ToList();
            }
            finally
            {
                this.StorageLock.ExitReadLock();
            }
        }

        public virtual void AddCommunicator(UserServiceCommunicator communicator)
        {
            if (communicator == null) return;
            this.Communicator = communicator;
        }

        public abstract void Save();

        public abstract void Initialize(); // get collection from xml file and get last generated Id
    }
}
