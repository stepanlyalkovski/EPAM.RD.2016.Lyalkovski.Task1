using System.Threading;

namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Entities;
    using Interfaces;
    using Interfaces.Repository;
    using Validation;

    public class MasterUserService : UserService
    {
        public MasterUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository)
            : this(numGenerator, validator, repository, false)
        {            
        }

        public MasterUserService(
            INumGenerator numGenerator,
            ValidatorBase<User> validator, 
            IRepository<User> repository, 
            bool loggingEnabled)
            : base(numGenerator, validator, repository, loggingEnabled)
        {  
        }

        public event EventHandler<UserDataApdatedEventArgs> Deleted;

        public event EventHandler<UserDataApdatedEventArgs> Added;

        public override void Save()
        {
            if (LoggingEnabled)
            {
                TraceSource.TraceEvent(TraceEventType.Information, 0, "Saving UserService state...");
            }

            Repository.Save(LastGeneratedId);
        }

        /// <summary>
        /// Initialize data from repository
        /// </summary>
        public override void Initialize()
        {
            if (LoggingEnabled)
            {
                TraceSource.TraceEvent(TraceEventType.Information, 0, "Initializing UserService state...");
            }

                try
                {
                Repository.Initialize();
                }
                catch (InvalidOperationException exception)
                {
                    if (LoggingEnabled)
                    {
                        TraceSource.TraceEvent(
                            TraceEventType.Error,
                            0,
                            "Services wasn't initialized! Exception message: " + exception.Message);
                    }
                }

            LastGeneratedId = Repository.GetState();
            NumGenerator.Initialize(LastGeneratedId);
                var users = Repository.GetAll().ToList();
            OnRepositoryClear(this, null);
                foreach (var user in users)
                {
                OnUserAdded(this, new UserDataApdatedEventArgs { User = user });
                }
        }

        protected override void DeleteStrategy(User user)
        {
            Repository.Delete(user);
            OnUserDeleted(this, new UserDataApdatedEventArgs { User = user });
        }

        protected override int AddStrategy(User user)
        {
            var errorMessages = Validator.Validate(user).ToList();
            if (errorMessages.Any())
            {
                TraceSource.TraceEvent(
                    TraceEventType.Error,
                    0,
              $"Validation ERROR on User: {user.LastName} {user.PersonalId}\n " + "ValidationMessages: " + string.Join("\n", errorMessages));
                throw new ArgumentException("Validation error! User is not valid\nValidationMessages: " + string.Join("\n", errorMessages));
            }

            user.Id = NumGenerator.GenerateId();
            LastGeneratedId = user.Id;
            Repository.Add(user);
            OnUserAdded(this, new UserDataApdatedEventArgs { User = user });
            return user.Id;
        }

        protected virtual void OnUserDeleted(object sender, UserDataApdatedEventArgs args)
        {
            Communicator?.SendDelete(args);
            Deleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            Communicator?.SendAdd(args);
            Added?.Invoke(sender, args);
        }

        protected virtual void OnRepositoryClear(object sender, EventArgs args)
        {
            Communicator?.SendClear(args);
        }
    }
}
