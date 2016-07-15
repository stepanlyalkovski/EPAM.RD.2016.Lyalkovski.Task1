using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Interfaces
{
    public interface IUserXmlFileWorker
    {
        void Save(SerializedUserData data, string filePath);
        SerializedUserData Load(string filePath);

    }
}
