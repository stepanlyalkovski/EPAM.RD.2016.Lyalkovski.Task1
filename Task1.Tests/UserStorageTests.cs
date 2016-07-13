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

namespace Task1.Tests
{
    [TestFixture]
    public class UserStorageTests
    {
        public UserStorage Storage { get; set; }
        public INumGenerator FakeNumGenerator { get; set; }
        public ValidatorBase<User> FakeValidator { get; set; }
        public IRepository<User> FakeRepository { get; set; }

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

            //var moqValidator = new Moq.Mock<ValidatorBase<User>>();
            //moqValidator.Setup(v => v.Validate(It.IsAny<User>())).Returns(new List<string>());
            //FakeValidator = moqValidator.Object;
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
            var userMemoryRepository = new MemoryRepository<User>();

            // stab for Num Generator
            // because we don't care what type of generator we will use in this test
            Storage = new UserStorage(FakeNumGenerator, validator, userMemoryRepository);

            int id = Storage.Add(validUser);
            var sameUser = Storage.Get(id);

            Assert.AreEqual(validUser, sameUser);
        }

        [Test]
        public void Add_AddFiveValidUsersToMemoryRepository_UsersExistInRepository()
        {
            ValidatorBase<User> validator = new SimpleUserValidator();
            var numGenerator = new EvenIdGenerator();
            var userMemoryRepository = new MemoryRepository<User>();
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
                var storageUser = Storage.Get(userId);
                Debug.WriteLine(storageUser.Id);
                Debug.WriteLine(storageUser.FirstName + " " + storageUser.LastName + " " + storageUser.BirthDate);
                receivedUsers.Add(storageUser);
            }

            Assert.AreEqual(userCount, receivedUsers.Count);
        }

        [Test]
        public void Delete_DeleteUserAndCheckTheStorage_StorageReturnedNull()
        {
            var userMemoryRepository = new MemoryRepository<User>();
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
            user = Storage.Get(userId);

            Assert.IsNull(user);
        }

        [Test]
        public void Delete_SendNullToDelete_IgnoreWithNoExceptions()
        {
            var userMemoryRepository = new MemoryRepository<User>();
            ValidatorBase<User> validator = new SimpleUserValidator();
            Storage = new UserStorage(FakeNumGenerator, validator, userMemoryRepository);
            Storage.Delete(null);

        }
    }


}