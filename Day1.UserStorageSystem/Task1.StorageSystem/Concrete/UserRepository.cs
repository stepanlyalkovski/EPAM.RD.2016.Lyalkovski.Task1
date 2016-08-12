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
            this.memoryCollection = new List<User>();
            this.xmlWorker = worker;
            this.filePath = filePath ?? Path.Combine(Directory.GetCurrentDirectory(), "testFile.xml");
        }

        public IEnumerable<int> SearhByPredicate(Func<User, bool>[] predicates)
        {
            return this.memoryCollection.Where(p => predicates.All(pr => pr(p))).Select(u => u.Id);
        }

        public IEnumerable<int> SearchByCriteria(ICriteria<User>[] criteries)
        {
            return this.memoryCollection.AsParallel().Where(u => criteries.All(cr => cr.MeetCriteria(u)))
                                    .Select(u => u.Id);
        }


        public void Add(User user)
        {
            var newUser = user.Clone();
            this.memoryCollection.Add(newUser);
        }

        public void Save(int lastGeneratedId)
        {
            var userData = new SerializedUserData
            {
                Users = this.memoryCollection.ToList(),
                LastGeneratedId = lastGeneratedId
            };
            this.SavetoXmlFile(userData);
        }

        public IEnumerable<User> GetAll()
        {
            return this.memoryCollection;
        }

        public int GetState()
        {
            return this.state;
        }

        public User GetById(int id)
        {
            return this.memoryCollection.FirstOrDefault(u => u.Id == id);
        }

        public void Delete(User entity)
        {
            this.memoryCollection.Remove(entity);
        }

        public void Clear()
        {
            this.memoryCollection.Clear();
        }

        public void Initialize()
        {
            this.InitializeFromXml();         
        }

        #region XMLworker
        private void SavetoXmlFile(SerializedUserData userData)
        {
            if (this.xmlWorker == null)
                throw new InvalidOperationException("xmlWorker wasn't initialized");

            this.xmlWorker.Save(userData, this.filePath);
        }

        private void InitializeFromXml()
        {
            if (this.xmlWorker == null)
                throw new InvalidOperationException("xmlWorker wasn't initialized");

            if (!File.Exists(this.filePath))
                throw new InvalidOperationException("File is not found");

            var xmlUsersCollection = this.xmlWorker.Load(this.filePath);
            var userData = new SerializedUserData();
            if (xmlUsersCollection != null)
                this.memoryCollection = xmlUsersCollection.Users;
            this.state = xmlUsersCollection.LastGeneratedId;
        }
#endregion
    }
}
