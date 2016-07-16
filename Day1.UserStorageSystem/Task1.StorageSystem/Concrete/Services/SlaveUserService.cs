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
        public SlaveUserService(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository) : base(numGenerator, validator, repository)
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