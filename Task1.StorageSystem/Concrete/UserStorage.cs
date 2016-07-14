using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete
{
    public class UserStorage
    {
        private INumGenerator _numGenerator;
        private IRepository<User> _repository;
        public int LastGeneratedId { get; private set; } //temp        
        public ValidatorBase<User> Validator { get; set; }
        public UserStorage(INumGenerator numGenerator, ValidatorBase<User> validator, IRepository<User> repository )
        {
            _numGenerator = numGenerator;
            Validator = validator;
            _repository = repository;
        }

        public int Add(User user)
        {
            var errorMessages = Validator.Validate(user).ToList();

            if (errorMessages.Any())
            {
                throw new ArgumentException("Entity is not valid:\n" + string.Join("\n", errorMessages));
            }
            
            user.Id = _numGenerator.GenerateId();
            LastGeneratedId = user.Id;
            _repository.Add(user);

            return user.Id;
        }

        public void Delete(User user)
        {
            
            _repository.Delete(user);
        }

        public User SearchForUser(Func<User, bool> predicate)
        {
            return  _repository.SearhByPredicate(predicate);
        }
    }
}
