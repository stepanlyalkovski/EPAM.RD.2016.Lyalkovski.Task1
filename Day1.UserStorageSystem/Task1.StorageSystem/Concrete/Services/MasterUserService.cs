using System;
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

        public MasterUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository) : base(numGenerator, validator, repository)
        {

        }

        public override int Add(User user)
        {
            var errorMessages = Validator.Validate(user).ToList();

            if (errorMessages.Any())
            {
                throw new ArgumentException("Entity is not valid:\n" + string.Join("\n", errorMessages));
            }

            user.Id = NumGenerator.GenerateId();
            LastGeneratedId = user.Id;
            Repository.Add(user);
            OnUserAdded(this, new UserDataApdatedEventArgs {User = user});
            return user.Id;
        }

        public override void Delete(User user)
        {
            Repository.Delete(user);
            OnUserDeleted(this, new UserDataApdatedEventArgs { User = user });
        }

        public override void Save()
        {
            Repository.Save(LastGeneratedId);
        }

        public override void Initialize()
        {
            Repository.Initialize();
            LastGeneratedId = Repository.GetState();
        }

        protected virtual void OnUserDeleted(object sender, UserDataApdatedEventArgs args)
        {
            Deleted?.Invoke(sender, args);
        }

        protected virtual void OnUserAdded(object sender, UserDataApdatedEventArgs args)
        {
            Added?.Invoke(sender, args);
        }
    }
}
