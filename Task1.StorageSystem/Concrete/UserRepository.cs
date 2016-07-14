using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.StorageSystem.Concrete
{
    public class UserRepository : IRepository<User>
    {
        private IList<User> _memoryCollection;
        private IUserXmlFileWorker _xmlWorker;
        private string filePath;
        public UserRepository(IUserXmlFileWorker worker, string filePath)
        {
            _memoryCollection = new List<User>();
            _xmlWorker = worker;
            this.filePath = filePath;
        }

        public IEnumerable<int> SearhByPredicate(Func<User, bool>[] predicates)
        {
            HashSet<int> ids = new HashSet<int>();
            return _memoryCollection.Where(p => predicates.Any(pr => pr(p))).Select(u => u.Id);
        }

        public void Add(User user)
        {
            _memoryCollection.Add(user);
        }

        public void Save()
        {
            SavetoXmlFile();
        }

        public void Delete(User entity)
        {
            _memoryCollection.Remove(entity);
        }

        public void Initialize()
        {
            InitializeFromXml();
        }

        #region XMLworker
        private void SavetoXmlFile()
        {
            _xmlWorker.Save(_memoryCollection.ToList(), filePath);
        }

        private void InitializeFromXml()
        {
            var xmlCollection = _xmlWorker.Load(filePath);

            if (xmlCollection != null)
                _memoryCollection = xmlCollection;
        }
#endregion
    }
}
