using System.Threading;

namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Validation;
    using Entities;
    using Interfaces;
    using Interfaces.Repository;

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
            if (this.LoggingEnabled)
            {
                this.TraceSource.TraceEvent(TraceEventType.Information, 0, "Saving UserService state...");
            }

            this.Repository.Save(this.LastGeneratedId);
        }

        public override void Initialize()
        {
            if (this.LoggingEnabled)
                this.TraceSource.TraceEvent(TraceEventType.Information, 0, "Initializing UserService state...");
                try
                {
                    this.Repository.Initialize();
                }
                catch (InvalidOperationException exception)
                {
                    if (this.LoggingEnabled)
                        this.TraceSource.TraceEvent(
                            TraceEventType.Error,
                            0,
                            "Services wasn't initialized! Exception message: " + exception.Message);
                }


                this.LastGeneratedId = this.Repository.GetState();
                this.NumGenerator.Initialize(this.LastGeneratedId);
                var users = this.Repository.GetAll().ToList();
                this.OnRepositoryClear(this, null);
                foreach (var user in users)
                {
                    this.OnUserAdded(this, new UserDataApdatedEventArgs { User = user });
                }

        }

        protected override void DeleteStrategy(User user)
        {
            this.Repository.Delete(user);
            this.OnUserDeleted(this, new UserDataApdatedEventArgs { User = user });
        }

        protected override int AddStrategy(User user)
        {
            var errorMessages = this.Validator.Validate(user).ToList();
            if (errorMessages.Any())
            {
                this.TraceSource.TraceEvent(
                    TraceEventType.Error,
                    0,
              $"Validation ERROR on User: {user.LastName} {user.PersonalId}\n " + "ValidationMessages: " + string.Join("\n", errorMessages));
                throw new ArgumentException("Validation error! User is not valid\nValidationMessages: " + string.Join("\n", errorMessages));
            }

            user.Id = this.NumGenerator.GenerateId();
            this.LastGeneratedId = user.Id;
            this.Repository.Add(user);
            this.OnUserAdded(this, new UserDataApdatedEventArgs { User = user });
            return user.Id;
        }

        protected virtual void OnUserDeleted(object sender, UserDataApdatedEventArgs args)
        {
            this.Communicator?.SendDelete(args);
            this.Deleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            this.Communicator?.SendAdd(args);
            this.Added?.Invoke(sender, args);
        }

        protected virtual void OnRepositoryClear(object sender, EventArgs args)
        {
            this.Communicator?.SendClear(args);
        }
    }
}
