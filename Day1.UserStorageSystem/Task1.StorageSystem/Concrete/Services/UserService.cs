using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading;
using Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;
namespace Task1.StorageSystem.Concrete.Services
{

    //[ServiceKnownType(typeof(MasterUserService))]
    //[ServiceKnownType(typeof(SlaveUserService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any)]
    [ServiceKnownType(typeof(ICriteria<User>))]
    [ServiceKnownType(typeof(CriterionMales))]
    public abstract class UserService : MarshalByRefObject, IUserServiceContract
    {
        protected INumGenerator NumGenerator;
        protected IRepository<User> Repository;                    
        protected TraceSource TraceSource;
        protected bool LoggingEnabled;
        protected ReaderWriterLockSlim storageLock = new ReaderWriterLockSlim();
        public ICriteria<User> criteria = new CriterionFemales(); 
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

                int id =  AddStrategy(user);

                if (LoggingEnabled)
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"User was added: {user.LastName} {user.PersonalId}. Id = {id}");
                return id;
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

                if (LoggingEnabled)
                    TraceSource.TraceEvent(TraceEventType.Information, 0, $"User {user.LastName} {user.PersonalId} was deleted");
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

        public virtual List<int> SearchForUsers(ICriteria<User>[] criteries)
        {
            storageLock.EnterReadLock();
            try
            {
                return Repository.SearchByCriteria(criteries).ToList();
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
