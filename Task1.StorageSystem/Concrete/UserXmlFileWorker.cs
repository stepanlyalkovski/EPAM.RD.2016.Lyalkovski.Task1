using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete
{
    public class UserXmlFileWorker : IUserXmlFileWorker
    {
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(SerializedUserData));
        public void Save(SerializedUserData data, string filePath)
        {
            using (Stream s = File.Create(filePath))
            {
                _serializer.Serialize(s, data);
            }
        }

        public SerializedUserData Load(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            using (Stream s = File.OpenRead(filePath))
            {
                var data = (SerializedUserData)_serializer.Deserialize(s);
                return data;
            }

        }
    }
}
