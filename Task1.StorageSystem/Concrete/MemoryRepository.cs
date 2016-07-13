using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete
{
    public class MemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private ICollection<T> _memoryCollection;

        public MemoryRepository()
        {
            _memoryCollection = new List<T>();
        }
        public T GetById(int id)
        {
            return _memoryCollection.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<T> List(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(T entity)
        {
            _memoryCollection.Add(entity);
        }

        public void Delete(T entity)
        {
            _memoryCollection.Remove(entity);
        }
    }
}
