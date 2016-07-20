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
        protected override int AddStrategy(User user)
        {
            Console.WriteLine(AppDomain.CurrentDomain.FriendlyName);
            //var errorMessages = Validator.Validate(user).ToList();
            //if (errorMessages.Any())
            //    throw new ArgumentException();
            Repository.Add(user);
            user.Id = NumGenerator.GenerateId();
            OnUserAdded(this, new UserDataApdatedEventArgs {User = user});
            return user.Id;
        }

        protected override void DeleteStrategy(User user)
        {
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
