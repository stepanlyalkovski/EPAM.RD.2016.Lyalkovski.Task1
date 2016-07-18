using System;
using System.Diagnostics;
using System.Linq;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.StorageSystem.Concrete.Services
{
    public class MasterUserService : UserService
    {
        public event EventHandler<UserDataApdatedEventArgs> Deleted;
        public event EventHandler<UserDataApdatedEventArgs> Added;

        public MasterUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository)
            :this(numGenerator, validator, repository, false)
        {

        }

        public MasterUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository, bool loggingEnabled)
                :base(numGenerator, validator, repository, loggingEnabled)
        {
            
        }
        public override int Add(User user)
        {
            var errorMessages = Validator.Validate(user).ToList();

            if (errorMessages.Any())
            {
                if (LoggingEnabled)
                    TraceSource.TraceEvent(TraceEventType.Error, 0, $"Invalid User. Validation messages: {string.Join("\n", errorMessages)}");
                throw new ArgumentException("Entity is not valid:\n" + string.Join("\n", errorMessages));

            }

            user.Id = NumGenerator.GenerateId();
            LastGeneratedId = user.Id;
            TraceSource.Listeners.Add(new ConsoleTraceListener());
            if (LoggingEnabled)
                TraceSource.TraceEvent(TraceEventType.Information, 0, $"Adding User: {user.LastName} {user.PersonalId}");

            Repository.Add(user);
            OnUserAdded(this, new UserDataApdatedEventArgs {User = user});
            return user.Id;
        }

        public override void Delete(User user)
        {
            if (LoggingEnabled)
                TraceSource.TraceEvent(TraceEventType.Information, 0, $"Deleting User: {user.LastName} {user.PersonalId}");

            Repository.Delete(user);
            OnUserDeleted(this, new UserDataApdatedEventArgs { User = user });
        }

        public override void Save()
        {
            if (LoggingEnabled)
                TraceSource.TraceEvent(TraceEventType.Information, 0, "Saving UserService state...");

            Repository.Save(LastGeneratedId);
        }

        public override void Initialize()
        {
            if (LoggingEnabled)
                TraceSource.TraceEvent(TraceEventType.Information, 0, "Initializing UserService state...");

            Repository.Initialize();
            LastGeneratedId = Repository.GetState();
        }

        protected virtual void OnUserDeleted(object sender, UserDataApdatedEventArgs args)
        {
            if (LoggingEnabled)
                TraceSource.TraceEvent(TraceEventType.Information, 0, $"User {args.User.LastName} {args.User.PersonalId} was deleted");

            Deleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            if (LoggingEnabled)
                TraceSource.TraceEvent(TraceEventType.Information, 0, $"User {args.User.LastName} {args.User.PersonalId} was added");

            Added?.Invoke(sender, args);
        }
    }
}
