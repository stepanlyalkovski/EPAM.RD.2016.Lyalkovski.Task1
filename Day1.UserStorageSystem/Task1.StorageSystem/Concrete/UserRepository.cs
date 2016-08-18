namespace Task1.StorageSystem.Concrete
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Entities;
    using Interfaces;
    using Interfaces.Repository;

    [Serializable]
    public class UserRepository : MarshalByRefObject, IRepository<User>
    {
        private IList<User> memoryCollection;
        private IUserXmlFileWorker xmlWorker;
        private string filePath;
        private int state; // we can add interface for State, but now it will be redundant

        public UserRepository(IUserXmlFileWorker worker, string filePath)
        {
            memoryCollection = new List<User>();
            xmlWorker = worker;
            this.filePath = filePath ?? Path.Combine(Directory.GetCurrentDirectory(), "testFile.xml");
        }

        public IEnumerable<int> SearhByPredicate(Func<User, bool>[] predicates)
        {
            return memoryCollection.Where(p => predicates.All(pr => pr(p))).Select(u => u.Id);
        }

        public IEnumerable<int> SearchByCriteria(ICriteria<User>[] criteries)
        {
            return memoryCollection.AsParallel().Where(u => criteries.All(cr => cr.MeetCriteria(u)))
                                    .Select(u => u.Id);
        }

        public void Add(User user)
        {
            var newUser = user.Clone();
            memoryCollection.Add(newUser);
        }

        public void Save(int lastGeneratedId)
        {
            var userData = new SerializedUserData
            {
                Users = memoryCollection.ToList(),
                LastGeneratedId = lastGeneratedId
            };
            SavetoXmlFile(userData);
        }

        public IEnumerable<User> GetAll()
        {
            return memoryCollection;
        }

        public int GetState()
        {
            return state;
        }

        public User GetById(int id)
        {
            return memoryCollection.FirstOrDefault(u => u.Id == id);
        }

        public void Delete(User entity)
        {
            memoryCollection.Remove(entity);
        }

        public void Clear()
        {
            memoryCollection.Clear();
        }

        public void Initialize()
        {
            InitializeFromXml();         
        }

        #region XMLworker
        private void SavetoXmlFile(SerializedUserData userData)
        {
            if (xmlWorker == null)
            {
                throw new InvalidOperationException("xmlWorker wasn't initialized");
            }

            xmlWorker.Save(userData, filePath);
        }

        private void InitializeFromXml()
        {
            if (xmlWorker == null)
            {
                throw new InvalidOperationException("xmlWorker wasn't initialized");
            }

            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("File is not found");
            }

            var xmlUsersCollection = xmlWorker.Load(filePath);
            var userData = new SerializedUserData();
            if (xmlUsersCollection != null)
            {
                memoryCollection = xmlUsersCollection.Users;
            }

            state = xmlUsersCollection.LastGeneratedId;
        }
#endregion
    }
}
