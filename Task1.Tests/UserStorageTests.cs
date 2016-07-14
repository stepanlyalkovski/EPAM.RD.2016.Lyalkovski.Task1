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
namespace Task1.Tests
{
    public class FolderElement : ConfigurationElement
    {

        [ConfigurationProperty("folderType", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string FolderType
        {
            get { return ((string)(base["folderType"])); }
            set { base["folderType"] = value; }
        }

        [ConfigurationProperty("path", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string Path
        {
            get { return ((string)(base["path"])); }
            set { base["path"] = value; }
        }
    }
    [ConfigurationCollection(typeof(FolderElement))]
    public class FoldersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new FolderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FolderElement)(element)).FolderType;
        }

        public FolderElement this[int idx]
        {
            get { return (FolderElement)BaseGet(idx); }
        }
    }
    public class StartupFoldersConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Folders")]
        public FoldersCollection FolderItems
        {
            get { return ((FoldersCollection)(base["Folders"])); }
        }
    }
    [TestFixture]
    public class UserStorageTests
    {
        public UserStorage Storage { get; set; }
        public INumGenerator FakeNumGenerator { get; set; }
        public ValidatorBase<User> FakeValidator { get; set; }
        public IRepository<User> FakeRepository { get; set; }

        private class EmptyUserValidator : ValidatorBase<User>
        {
            protected override IEnumerable<Rule> Rules => new List<Rule>();
        }

        public UserStorageTests()
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

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_AddInvalidUser_ThrownArgumentExceptionWithValidationMessages()
        {
            ValidatorBase<User> validator = new SimpleUserValidator();
            Storage = new UserStorage(FakeNumGenerator, validator, FakeRepository);

            var invalidUser = new User
            {
                FirstName = "Ivan",
                LastName = null,
                BirthDate = new DateTime()
            };

            Storage.Add(invalidUser);
        }

        [Test]
        public void Add_AddValidUserToMemoryRepositoryAndThanGetItByReceivedId_StorageReturnedCurrentUser()
        {

            ValidatorBase<User> validator = new SimpleUserValidator();
            var validUser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                BirthDate = DateTime.Now
            };
            var userMemoryRepository = new UserRepository();

            // stab for Num Generator
            // because we don't care what type of generator we will use in this test
            Storage = new UserStorage(FakeNumGenerator, validator, userMemoryRepository);

            int id = Storage.Add(validUser);
            var sameUser = Storage.SearchForUser(u => u.Id == id);

            Assert.AreEqual(validUser, sameUser);
        }

        [Test]
        public void Add_AddFiveValidUsersToMemoryRepository_UsersExistInRepository()
        {
            ValidatorBase<User> validator = new SimpleUserValidator();
            var numGenerator = new EvenIdGenerator();
            var userMemoryRepository = new UserRepository();
            int userCount = 5;
            Storage = new UserStorage(numGenerator, validator, userMemoryRepository);
            var receivedUsers = new List<User>();

            for (int i = 0; i < userCount; i++)
            {
                var validUser = new User
                {
                    FirstName = "generated",
                    LastName = $"User {i + 1}",
                    BirthDate = DateTime.Now
                };

                int userId = Storage.Add(validUser);
                var storageUser = Storage.SearchForUser(u => u.Id == userId);
                Debug.WriteLine(storageUser.Id);
                Debug.WriteLine(storageUser.FirstName + " " + storageUser.LastName + " " + storageUser.BirthDate);
                receivedUsers.Add(storageUser);
            }

            Assert.AreEqual(userCount, receivedUsers.Count);
        }

        [Test]
        public void Delete_DeleteUserAndCheckTheStorage_StorageReturnedNull()
        {
            var userMemoryRepository = new UserRepository();
            ValidatorBase<User> validator = new SimpleUserValidator();
            Storage = new UserStorage(FakeNumGenerator, validator, userMemoryRepository);
            var user = new User
            {
                FirstName = "Default",
                LastName = "Default",
                BirthDate = DateTime.Now
            };

            int userId = Storage.Add(user);
            Storage.Delete(user);
            user = Storage.SearchForUser(u => u.Id == userId);

            Assert.IsNull(user);
        }

        [Test]
        public void Delete_SendNullToDelete_IgnoreWithNoExceptions()
        {
            var userMemoryRepository = new UserRepository();
            ValidatorBase<User> validator = new SimpleUserValidator();
            Storage = new UserStorage(FakeNumGenerator, validator, userMemoryRepository);
            Storage.Delete(null);

        }

        [Test]
        public void SearchForUser_SearchUserByLastNameAndPersonalId_UserIsFoundAndEqualToRequired()
        {
            var requiredUser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP12345",             
            };
            var anotherUser = new User
            {
                FirstName = "NOT_Ivan",
                LastName = "Ivanov",
                PersonalId = "MP5678"
            };
            Storage = new UserStorage(new EvenIdGenerator(),  new EmptyUserValidator(), new UserRepository());
            Storage.Add(requiredUser);
            Storage.Add(anotherUser);

            var searchUser = Storage.SearchForUser(u => u.LastName == "Ivanov" && u.PersonalId == "MP12345");
            
            Assert.AreEqual(requiredUser, searchUser);
        }

        [Test]
        public void AddToXmlStorage_Test()
        {
            //var userXmlRepository = new XmlFileRepository();
            //ValidatorBase<User> validator = new SimpleUserValidator();
            //var user = new User
            //{
            //    FirstName = "AnotherUSer",
            //    LastName = "Default",
            //    BirthDate = DateTime.Now,
            //    PersonalId = "test"
            //};
            //Storage = new UserStorage(new EvenIdGenerator(), validator, userXmlRepository);

            //Storage.Add(user);
            //StartupFoldersConfigSection section = (StartupFoldersConfigSection)ConfigurationManager.GetSection("StartupFolders");

            var userMemoryRepository = new UserRepository();
            ValidatorBase<User> validator = new SimpleUserValidator();
            int lastId = 16;
            Storage = new UserStorage(new EvenIdGenerator(lastId), validator, userMemoryRepository);
            var firstUser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP123",
                BirthDate = DateTime.Now
            };

            var secondUser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP123",
                BirthDate = DateTime.Now
            };
            Storage.Add(secondUser);
            //Storage.Add(firstUser);
            var user = Storage.SearchForUser(u => u.FirstName == "Ivan" && u.LastName == "Ivanov");
            if(user != null)
                Debug.WriteLine(user.LastName + " " + user.Id);
        }
    }


}