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
using ServiceConfigurator;
using ServiceConfigurator.CustomSections.Files;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.Tests
{
    public class EmptyUserValidator : ValidatorBase<User>
    {
        protected override IEnumerable<Rule> Rules => new List<Rule>();
    }


    [TestFixture]
    public class UserServiceTests
    {

        private UserService Service { get; set; }
        public User SimpleUser { get; set; } = new User
        {
            FirstName = "Ivan2",
            LastName = "Ivanov2",
            PersonalId = "MP12345",
            BirthDate = DateTime.Now,
        };

        //Using Moq to create fake entities
        private INumGenerator FakeNumGenerator { get; set; }
        private IRepository<User> FakeRepository { get; set; }
        private EmptyUserValidator FakeValidator { get; set; }
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
            FakeValidator = new EmptyUserValidator();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_AddInvalidUser_ThrownArgumentExceptionWithValidationMessages()
        {
            ValidatorBase<User> validator = new SimpleUserValidator();
            Service = new MasterUserService(FakeNumGenerator, validator, FakeRepository);

            var invalidUser = new User
            {
                FirstName = "Ivan",
                LastName = null,
                BirthDate = new DateTime()
            };

            Service.Add(invalidUser);
        }

        [Test]
        public void Add_AddValidUserToMemoryRepositoryAndThanGetItBySearch_StorageReturnedCurrentUserId()
        {

            ValidatorBase<User> validator = new SimpleUserValidator();
            var validUser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP123",
                BirthDate = DateTime.Now
            };
            var userMemoryRepository = new UserRepository(null, null); // we don't need it

            // because we don't care what type of generator we will use in this test
            Service = new MasterUserService(FakeNumGenerator, validator, userMemoryRepository);

            int id = Service.Add(validUser);
            var recievedId = Service.SearchForUsers(new Func<User, bool>[]
            {
                u => u.PersonalId == validUser.PersonalId
            }).FirstOrDefault();

            Assert.AreEqual(id, recievedId);
        }

        [Test]
        public void Add_AddFiveValidUsersToMemoryRepository_ThereAreFiveUsersInRepository()
        {
            ValidatorBase<User> validator = new SimpleUserValidator();
            var numGenerator = new EvenIdGenerator();
            var userMemoryRepository = new UserRepository(null, null);
            int userCount = 5;
            Service = new MasterUserService(numGenerator, validator, userMemoryRepository);
            var receivedUsers = new List<User>();

            for (int i = 0; i < userCount; i++)
            {
                var validUser = new User
                {
                    FirstName = "generated",
                    LastName = $"User {i + 1}",
                    BirthDate = DateTime.Now
                };

                int userId = Service.Add(validUser);
            }

            var storageUserCount = Service.SearchForUsers(new Func<User, bool>[]
            {
                u => !u.LastName.IsNullOrEmpty() // kind of GetAll -_-
            }).Count();

            Assert.AreEqual(userCount, storageUserCount);
        }

        [Test]
        public void Delete_DeleteUserAndCheckTheStorage_StorageReturnedNull()
        {
            var userMemoryRepository = new UserRepository(null, null);
            ValidatorBase<User> validator = new SimpleUserValidator();
            Service = new MasterUserService(FakeNumGenerator, validator, userMemoryRepository);
            var user = new User
            {
                FirstName = "Default",
                LastName = "Default",
                BirthDate = DateTime.Now
            };

            int userId = Service.Add(user);
            Service.Delete(user);
            var users = Service.SearchForUsers(new Func<User, bool>[] { u => u.Id == userId });

            Assert.IsEmpty(users);
        }

        [Test]
        public void Delete_SendNullToDelete_IgnoreWithNoExceptions()
        {
            var userMemoryRepository = new UserRepository(null, null);
            ValidatorBase<User> validator = new SimpleUserValidator();
            Service = new MasterUserService(FakeNumGenerator, validator, userMemoryRepository);
            Service.Delete(null);

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
            Service = new MasterUserService(new EvenIdGenerator(), new EmptyUserValidator(),
                                                new UserRepository(null, null));
            int requiredId = Service.Add(requiredUser);
            Service.Add(anotherUser);

            var searchUserId = Service.SearchForUsers(new Func<User, bool>[]
            {
                u => u.LastName == "Ivanov" && u.PersonalId == "MP12345"
            }).FirstOrDefault();

            Assert.AreEqual(requiredId, searchUserId);
        }

        [Test]
        public void ConfigTest()
        {
            string filePath = FileInitializer.GetFilePath();
            var userMemoryRepository = new UserRepository(new UserXmlFileWorker(), filePath);
            ValidatorBase<User> validator = new SimpleUserValidator();
            int lastId = 20;
            Service = new MasterUserService(new EvenIdGenerator(lastId), validator, userMemoryRepository);
            VisaRecord record = new VisaRecord
            {
                Country = "someCountry",
                StartDate = DateTime.MinValue,
                EndDate = DateTime.Now
            };
            var firstUser = new User
            {
                FirstName = "Ivan2",
                LastName = "Ivanov2",
                PersonalId = "MP12345",
                BirthDate = DateTime.Now,
                VisaRecords = new List<VisaRecord> { record }
            };

            Service.Add(firstUser);
            Service.Save();
        }

        [Test]
        public void Initialize_GetUsersFromXmlWithLastGeneratedId_ReturnedProperId()
        {
            string filePath = "D://forTests.xml";
            var userMemoryRepository = new UserRepository(new UserXmlFileWorker(), filePath);
            ValidatorBase<User> validator = new SimpleUserValidator();
            int lastId = 10;
            int expectedId = 14; // not 12 because we will add one user
            Service = new MasterUserService(new EvenIdGenerator(lastId), validator, userMemoryRepository);
            Service.Add(SimpleUser);
            Service.Save();
            var anotheruser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP1",
                BirthDate = DateTime.Now,
            };

            Service.Initialize();
            int id = Service.Add(anotheruser);

            Debug.WriteLine(id);
            Assert.AreEqual(expectedId, id);
        }

        [Test]
        public void Initialize_SaveUserAndThanInitilizeRepository_ReturnedEqualUserId()
        {
            string filePath = "D://forTests.xml";
            var userMemoryRepository = new UserRepository(new UserXmlFileWorker(), filePath);
            ValidatorBase<User> validator = new SimpleUserValidator();
            Service = new MasterUserService(new EvenIdGenerator(), validator, userMemoryRepository);
            int expectedId = Service.Add(SimpleUser);
            Service.Save();
            Service.Initialize();
            var predicates = new Func<User, bool>[] { p => p.PersonalId == SimpleUser.PersonalId };
            var user = Service.SearchForUsers(predicates).First();
            Assert.AreEqual(expectedId, user);
        }

        // Trying to find way to test Deep Clone
        [Test]
        public void Add_EditAddedUser_UserInRepositoryWasNotEdited()
        {
            var userRepository = new UserRepository(null, null);
            ValidatorBase<User> validator = new SimpleUserValidator();
            Service = new MasterUserService(new EvenIdGenerator(), validator, userRepository);
            int id = Service.Add(SimpleUser);

            SimpleUser.Id = id + 1; // change User ID in local object

            int userRepositoryId = userRepository.SearhByPredicate(new Func<User, bool>[]
            {
                u => u.PersonalId == SimpleUser.PersonalId
            }).First();
            Debug.WriteLine("UserRepositoryId:" + userRepositoryId);
            Assert.AreNotEqual(SimpleUser.Id, userRepositoryId);
        }
    }

}