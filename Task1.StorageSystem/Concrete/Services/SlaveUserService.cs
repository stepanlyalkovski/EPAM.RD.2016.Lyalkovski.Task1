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

        private void UpdateData(object sender, EventArgs args)
        {
            Debug.WriteLine("Slave has received edit notification");
        }

        public void Subscribe(MasterUserService master)
        {
            master.WasEdited += UpdateData;
        }
    }
}