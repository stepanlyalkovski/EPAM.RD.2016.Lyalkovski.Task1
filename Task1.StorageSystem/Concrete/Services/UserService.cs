using System;
using System.Collections.Generic;
using System.Linq;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.StorageSystem.Concrete.Services
{
    public abstract class UserService
    {
        protected INumGenerator NumGenerator;
        protected IRepository<User> Repository;
        public int LastGeneratedId { get; protected set; } //temp        
        public ValidatorBase<User> Validator { get; set; }

        protected UserService(INumGenerator numGenerator, ValidatorBase<User> validator, 
                                IRepository<User> repository)
        {
            NumGenerator = numGenerator;
            Validator = validator;
            Repository = repository;
        }
        public abstract int Add(User user);
        public abstract void Delete(User user);

        public virtual IEnumerable<int> SearchForUsers(Func<User, bool>[] predicates)
        {
            return Repository.SearhByPredicate(predicates).ToList();
        }

        public abstract void Save();
        public abstract void Initialize(); // get collection from xml file and get last generated Id

    }
}
