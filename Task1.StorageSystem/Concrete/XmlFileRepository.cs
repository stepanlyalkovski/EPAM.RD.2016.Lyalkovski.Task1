using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete
{
    public class XmlFileRepository : IRepository<User>
    {
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
        private string tempFilePath = @"D:\test.xml";
        private IList<User> _users = new List<User>();
        public User GetById(int id)
        {
            throw new NotImplementedException();
        }

        public User SearhByPredicate(Func<User, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> List(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        private void ReadFile()
        {
            if (File.Exists(tempFilePath))
            {
                using (Stream s = File.OpenRead(tempFilePath))
                {

                    _users = (IList<User>)serializer.Deserialize(s);
                }
            }
        }

        public void Add(User user)
        {
            ReadFile();
            _users.Add(user);
            using (Stream s = File.Create(tempFilePath))
            {
                serializer.Serialize(s, _users);
            }
        }

        public void Delete(User entity)
        {
            throw new NotImplementedException();
        }
    }
}
