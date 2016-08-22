using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Entities;

namespace Task1.Tests
{
    [TestFixture]
    public class UserXmlFileWorkerTests
    {
        [Test]
        public void SaveAndLoad_SaveUserDataAndThanLoad_ReturnedEqualData()
        {
            string filePath = "D://forTests.xml";
            var xmlWorker = new UserXmlFileWorker();
            SerializedUserData userData = new SerializedUserData
            {
                LastGeneratedId = 0,
                Users = new List<User>
                {
                    new User { LastName = "Ivanov" },
                    new User { LastName = "Petrov" }
                }
            };
            
            xmlWorker.Save(userData, filePath);
            var loadedData = xmlWorker.Load(filePath);
            bool isEqual = loadedData.LastGeneratedId == userData.LastGeneratedId
                           && loadedData.Users.SequenceEqual(userData.Users);

            Assert.IsTrue(isEqual);
        }
    }
}