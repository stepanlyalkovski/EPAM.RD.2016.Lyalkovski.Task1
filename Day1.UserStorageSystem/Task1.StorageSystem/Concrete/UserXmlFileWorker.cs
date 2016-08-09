namespace Task1.StorageSystem.Concrete
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Entities;
    using Interfaces;

    [Serializable]
    public class UserXmlFileWorker : MarshalByRefObject, IUserXmlFileWorker, ISerializable
    {
        private readonly XmlSerializer serializer;

        public UserXmlFileWorker()
        {
            this.serializer = new XmlSerializer(typeof(SerializedUserData));
        }
        public UserXmlFileWorker(SerializationInfo info, StreamingContext context)
        {
            var type = Type.GetType((string)info.GetValue("typeName", typeof(string)));
            this.serializer = new XmlSerializer(type);
        }

        public void Save(SerializedUserData data, string filePath)
        {
            using (Stream s = File.Create(filePath))
            {
                this.serializer.Serialize(s, data);
            }
        }

        public SerializedUserData Load(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            using (Stream s = File.OpenRead(filePath))
            {
                var data = (SerializedUserData)this.serializer.Deserialize(s);
                return data;
            }

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("typeName", typeof(SerializedUserData).FullName, typeof(string));
        }
    }
}
