using System;
using System.Diagnostics;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.StorageSystem.Concrete.Services
{
    public class SlaveUserService : UserService
    {
        public SlaveUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository) 
            : this(numGenerator, validator, repository, false)
        {
        }
        public SlaveUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository, bool loggingEnabled) 
                        : base(numGenerator, validator, repository, loggingEnabled)
        {
        }
        public override int Add(User user)
        {
            throw new NotSupportedException();
        }

        public override void Delete(User user)
        {
            throw new NotSupportedException();
        }

        public override void Save()
        {
            throw new NotSupportedException();
        }

        public override void Initialize()
        {
            throw new NotSupportedException();
        }

        private void OnAdded(object sender, UserDataApdatedEventArgs args)
        {
            Repository.Add(args.User);
            LastGeneratedId = args.User.Id;
        }

        private void OnDeleted(object sender, UserDataApdatedEventArgs args)
        {
            Repository.Delete(args.User);
        }

        public void Subscribe(MasterUserService master)
        {
            master.Deleted += OnDeleted;
            master.Added += OnAdded;
        }
    }
}