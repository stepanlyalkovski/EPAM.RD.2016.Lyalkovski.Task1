using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete
{
    public class UserRepository : IRepository<User>
    {
        private ICollection<User> _memoryCollection;

        public UserRepository()
        {
            _memoryCollection = new List<User>();
        }

        public User SearhByPredicate(Func<User, bool> predicate)
        {
            return _memoryCollection.Where(predicate).FirstOrDefault();
        }

        public void Add(User user)
        {
            _memoryCollection.Add(user);
        }

        public void SaveToXmlFile()
        {
            throw new NotImplementedException();
        }

        public void Delete(User entity)
        {
            _memoryCollection.Remove(entity);
        }
    }
}
