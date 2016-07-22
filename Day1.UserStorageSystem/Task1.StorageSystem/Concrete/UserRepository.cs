using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.StorageSystem.Concrete
{
    [Serializable]
    public class UserRepository : MarshalByRefObject, IRepository<User>
    {
        private IList<User> _memoryCollection;
        private IUserXmlFileWorker _xmlWorker;
        private string filePath;
        private int _state; // we can add interface for State, but now it will be redundant
        public UserRepository(IUserXmlFileWorker worker, string filePath)
        {
            _memoryCollection = new List<User>();
            _xmlWorker = worker;
            this.filePath = filePath ?? Path.Combine(Directory.GetCurrentDirectory(),"testFile.xml");
        }

        public IEnumerable<int> SearhByPredicate(Func<User, bool>[] predicates)
        {
            return _memoryCollection.Where(p => predicates.Any(pr => pr(p))).Select(u => u.Id);
        }

        public void Add(User user)
        {
            var newUser = user.Clone();
            _memoryCollection.Add(newUser);
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

        public User GetById(int id)
        {
            return _memoryCollection.FirstOrDefault(u => u.Id == id);
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
            if(_xmlWorker == null)
                throw new InvalidOperationException("xmlWorker wasn't initialized");

            _xmlWorker.Save(userData, filePath);
        }

        private void InitializeFromXml()
        {
            if (_xmlWorker == null)
                throw new InvalidOperationException("xmlWorker wasn't initialized");

            var xmlUsersCollection = _xmlWorker.Load(filePath);
            var userData = new SerializedUserData();
            if (xmlUsersCollection != null)
               _memoryCollection = xmlUsersCollection.Users;
            _state = xmlUsersCollection.LastGeneratedId;
        }
#endregion
    }
}
