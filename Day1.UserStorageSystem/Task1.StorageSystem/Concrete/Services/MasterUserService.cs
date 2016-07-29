using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using NetworkServiceCommunication;
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
            var errorMessages = Validator.Validate(user).ToList();
            if (errorMessages.Any())
            {
                TraceSource.TraceEvent(TraceEventType.Error, 0, $"Validation ERROR on User: {user.LastName} {user.PersonalId}\n " +
                                                                "ValidationMessages: " + string.Join("\n",errorMessages));
                throw new ArgumentException("Validation error! User is not valid\nValidationMessages: " + string.Join("\n", errorMessages));
            }
            //if (UserExists(user))
            //{
            //    throw new ArgumentException("That User was added before!");
            //}
            user.Id = NumGenerator.GenerateId();
            Repository.Add(user);
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
            //if (LoggingEnabled)
            //    TraceSource.TraceEvent(TraceEventType.Information, 0, $"User {args.User.LastName} {args.User.PersonalId} was deleted");
            Communicator?.SendDelete(args);
            Deleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            //if (LoggingEnabled)
            //    TraceSource.TraceEvent(TraceEventType.Information, 0, $"User {args.User.LastName} {args.User.PersonalId} was added");
            Communicator?.SendAdd(args);
            Added?.Invoke(sender, args);
        }

        private bool UserExists(User user)
        {
            return Repository.SearhByPredicate(new Func<User, bool>[]
            {
                u => u.PersonalId == user.PersonalId
            }).Any();
        }
    }
}
