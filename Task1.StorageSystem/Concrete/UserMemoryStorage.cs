using System.Collections;
using System.Collections.Generic;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete
{
    public class UserMemoryStorage : IUserStorage
    {
        private INumGenerator _numGenerator;
        private IList<User> _users;
        public UserMemoryStorage(INumGenerator numGenerator)
        {
            this._numGenerator = numGenerator;
            _users = new List<User>();
        }

        public int Add(User user)
        {
            //validate

            user.Id = _numGenerator.GenerateId();

            //Memory way
            //TODO create some inteface or use Repository
            _users.Add(user);

            return user.Id;
        }

        public void Delete(User user)
        {
            throw new System.NotImplementedException();
        }
    }
}