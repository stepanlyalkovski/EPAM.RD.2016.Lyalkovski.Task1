using System;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Components.DictionaryAdapter.Xml;
using Moq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.IdGenerator;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using System.Configuration;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using ConfigGenerator.FileConfigurator;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.Tests
{
//    #region test AppConfig

//    [ConfigurationCollection(typeof(FileElement))]
//    public class FoldersCollection : ConfigurationElementCollection
//    {
//        protected override ConfigurationElement CreateNewElement()
//        {
//            return new FileElement();
//        }

//        protected override object GetElementKey(ConfigurationElement element)
//        {
//            return ((FileElement)(element)).FileType;
//        }

//        public FileElement this[int idx] => (FileElement)BaseGet(idx);
//    }
//    public class StartupFoldersConfigSection : ConfigurationSection
//    {
//        [ConfigurationProperty("Folders")]
//        public FoldersCollection FolderItems => (FoldersCollection)base["Folders"];
//    }

//#endregion


    [TestFixture]
    public class UserServiceTests
    {
        public UserService Service { get; set; }
        public INumGenerator FakeNumGenerator { get; set; }
        public ValidatorBase<User> FakeValidator { get; set; }
        public IRepository<User> FakeRepository { get; set; }

        private class EmptyUserValidator : ValidatorBase<User>
        {
            protected override IEnumerable<Rule> Rules => new List<Rule>();
        }
        public User SimpleUser { get; set;} = new User
        {
            FirstName = "Ivan2",
            LastName = "Ivanov2",
            PersonalId = "MP12345",
            BirthDate = DateTime.Now,
        };

        public UserServiceTests()
        {
            int fakeId = 1;
            var moqGenerator = new Moq.Mock<INumGenerator>();
            moqGenerator.Setup(g => g.GenerateId()).Returns(fakeId);
            FakeNumGenerator = moqGenerator.Object;

            var moqRepository = new Moq.Mock<IRepository<User>>();
            // stab for repository
            moqRepository.Setup(r => r.Add(It.IsAny<User>()));
            FakeRepository = moqRepository.Object;
        }

        //[Test]
        //[ExpectedException(typeof(ArgumentException))]
        //public void Add_AddInvalidUser_ThrownArgumentExceptionWithValidationMessages()
        //{
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    Service = new UserService(FakeNumGenerator, validator, FakeRepository);

        //    var invalidUser = new User
        //    {
        //        FirstName = "Ivan",
        //        LastName = null,
        //        BirthDate = new DateTime()
        //    };

        //    Service.Add(invalidUser);
        //}

        //[Test]
        //public void Add_AddValidUserToMemoryRepositoryAndThanGetItByReceivedId_StorageReturnedCurrentUser()
        //{

        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    var validUser = new User
        //    {
        //        FirstName = "Ivan",
        //        LastName = "Ivanov",
        //        BirthDate = DateTime.Now
        //    };
        //    var userMemoryRepository = new UserRepository();

        //    // stab for Num Generator
        //    // because we don't care what type of generator we will use in this test
        //    Service = new UserService(FakeNumGenerator, validator, userMemoryRepository);

        //    int id = Service.Add(validUser);
        //    var sameUser = Service.SearchForUsers(u => u.Id == id);

        //    Assert.AreEqual(validUser, sameUser);
        //}

        //[Test]
        //public void Add_AddFiveValidUsersToMemoryRepository_UsersExistInRepository()
        //{
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    var numGenerator = new EvenIdGenerator();
        //    var userMemoryRepository = new UserRepository();
        //    int userCount = 5;
        //    Service = new UserService(numGenerator, validator, userMemoryRepository);
        //    var receivedUsers = new List<User>();

        //    for (int i = 0; i < userCount; i++)
        //    {
        //        var validUser = new User
        //        {
        //            FirstName = "generated",
        //            LastName = $"User {i + 1}",
        //            BirthDate = DateTime.Now
        //        };

        //        int userId = Service.Add(validUser);
        //        var storageUser = Service.SearchForUsers(u => u.Id == userId);
        //        Debug.WriteLine(storageUser.Id);
        //        Debug.WriteLine(storageUser.FirstName + " " + storageUser.LastName + " " + storageUser.BirthDate);
        //        receivedUsers.Add(storageUser);
        //    }

        //    Assert.AreEqual(userCount, receivedUsers.Count);
        //}

        //[Test]
        //public void Delete_DeleteUserAndCheckTheStorage_StorageReturnedNull()
        //{
        //    var userMemoryRepository = new UserRepository();
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    Service = new UserService(FakeNumGenerator, validator, userMemoryRepository);
        //    var user = new User
        //    {
        //        FirstName = "Default",
        //        LastName = "Default",
        //        BirthDate = DateTime.Now
        //    };

        //    int userId = Service.Add(user);
        //    Service.Delete(user);
        //    user = Service.SearchForUsers(u => u.Id == userId);

        //    Assert.IsNull(user);
        //}

        //[Test]
        //public void Delete_SendNullToDelete_IgnoreWithNoExceptions()
        //{
        //    var userMemoryRepository = new UserRepository();
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    Service = new UserService(FakeNumGenerator, validator, userMemoryRepository);
        //    Service.Delete(null);

        //}

        //[Test]
        //public void SearchForUser_SearchUserByLastNameAndPersonalId_UserIsFoundAndEqualToRequired()
        //{
        //    var requiredUser = new User
        //    {
        //        FirstName = "Ivan",
        //        LastName = "Ivanov",
        //        PersonalId = "MP12345",             
        //    };
        //    var anotherUser = new User
        //    {
        //        FirstName = "NOT_Ivan",
        //        LastName = "Ivanov",
        //        PersonalId = "MP5678"
        //    };
        //    Service = new UserService(new EvenIdGenerator(),  new EmptyUserValidator(), new UserRepository());
        //    Service.Add(requiredUser);
        //    Service.Add(anotherUser);

        //    var searchUser = Service.SearchForUsers(u => u.LastName == "Ivanov" && u.PersonalId == "MP12345");
            
        //    Assert.AreEqual(requiredUser, searchUser);
        //}

        //[Test]
        //public void AddToXmlStorage_Test()
        //{
        //    //var userXmlRepository = new XmlFileRepository();
        //    //ValidatorBase<User> validator = new SimpleUserValidator();
        //    //var user = new User
        //    //{
        //    //    FirstName = "AnotherUSer",
        //    //    LastName = "Default",
        //    //    BirthDate = DateTime.Now,
        //    //    PersonalId = "test"
        //    //};
        //    //Service = new UserService(new EvenIdGenerator(), validator, userXmlRepository);

        //    //Service.Add(user);
        //    //StartupFoldersConfigSection section = (StartupFoldersConfigSection)ConfigurationManager.GetSection("StartupFolders");

        //    var userMemoryRepository = new UserRepository();
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    int lastId = 16;
        //    Service = new UserService(new EvenIdGenerator(lastId), validator, userMemoryRepository);
        //    var SimpleUser = new User
        //    {
        //        FirstName = "Ivan",
        //        LastName = "Ivanov",
        //        PersonalId = "MP123",
        //        BirthDate = DateTime.Now
        //    };

        //    var secondUser = new User
        //    {
        //        FirstName = "Ivan",
        //        LastName = "Ivanov",
        //        PersonalId = "MP123",
        //        BirthDate = DateTime.Now
        //    };
        //    Service.Add(secondUser);
        //    //Service.Add(SimpleUser);
        //    var user = Service.SearchForUsers(u => u.FirstName == "Ivan" && u.LastName == "Ivanov");
        //    if(user != null)
        //        Debug.WriteLine(user.LastName + " " + user.Id);
        //}

        //[Test]
        //public void ConfigTest()
        //{         
        //    string filePath = TempFileInitializer.GetFilePath((StartupFilesConfigSection)ConfigurationManager.GetSection("StartupFiles"));
        //    var userMemoryRepository = new UserRepository(new UserXmlFileWorker(), filePath);
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    int lastId = 20;
        //    Service = new UserService(new EvenIdGenerator(lastId), validator, userMemoryRepository);
        //    VisaRecord record = new VisaRecord
        //    {
        //        Country = "someCountry",
        //        StartDate = DateTime.MinValue,
        //        EndDate = DateTime.Now
        //    };
        //    var firstUser = new User
        //    {
        //        FirstName = "Ivan2",
        //        LastName = "Ivanov2",
        //        PersonalId = "MP12345",
        //        BirthDate = DateTime.Now,
        //        VisaRecords = new List<VisaRecord> { record}               
        //    };

        //    Service.Add(firstUser);
        //    Service.Save();
        //}

        //[Test]
        //public void Initialize_GetUsersFromXmlWithLastGeneratedId_ReturnedProperId()
        //{
        //    string filePath = "D://forTests.xml";
        //    var userMemoryRepository = new UserRepository(new UserXmlFileWorker(), filePath);
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    int lastId = 10;
        //    int expectedId = 14; // not 12 because we will add one user
        //    Service = new UserService(new EvenIdGenerator(lastId), validator, userMemoryRepository);
        //    Service.Add(SimpleUser);
        //    Service.Save();
        //    var anotheruser = new User
        //    {
        //        FirstName = "Ivan",
        //        LastName = "Ivanov",
        //        PersonalId = "MP1",
        //        BirthDate = DateTime.Now,
        //    };

        //    Service.Initialize();
        //    int id = Service.Add(anotheruser);

        //    Debug.WriteLine(id);
        //    Assert.AreEqual(expectedId, id);
        //}

        //[Test]
        //public void Initialize_SaveUserAndThanInitilizeRepository_ReturnedEqualUserId()
        //{
        //    string filePath = "D://forTests.xml";
        //    var userMemoryRepository = new UserRepository(new UserXmlFileWorker(), filePath);
        //    ValidatorBase<User> validator = new SimpleUserValidator();
        //    Service = new UserService(new EvenIdGenerator(), validator, userMemoryRepository);
        //    int expectedId = Service.Add(SimpleUser);
        //    Service.Save();
        //    Service.Initialize();
        //    var predicates = new Func<User, bool>[] {p => p.PersonalId == SimpleUser.PersonalId };
        //    var user = Service.SearchForUsers(predicates).First();
        //    Assert.AreEqual(expectedId, user);
        //}
    }

}