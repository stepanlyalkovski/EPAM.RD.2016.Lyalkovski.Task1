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
        private int _state; // we can add interface for State, but now it will be redundant
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

        public void Save(int lastGeneratedId)
        {
            var userData = new SerializedUserData
            {
                Users = _memoryCollection.ToList(),
                LastGeneratedId = lastGeneratedId
            };
            SavetoXmlFile(userData);
        }

        public int GetState()
        {
            return _state;
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
        private void SavetoXmlFile(SerializedUserData userData)
        {
            _xmlWorker.Save(userData, filePath);
        }

        private void InitializeFromXml()
        {
            var xmlUsersCollection = _xmlWorker.Load(filePath);
            var userData = new SerializedUserData();
            if (xmlUsersCollection != null)
               _memoryCollection = xmlUsersCollection.Users;
            _state = xmlUsersCollection.LastGeneratedId;
        }
#endregion
    }
}
