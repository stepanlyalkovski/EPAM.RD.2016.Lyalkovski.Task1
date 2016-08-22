using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Castle.Core.Internal;
using Moq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.IdGenerator;
using Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        public UserServiceTests()
        {
            int fakeId = 1;
            var moqGenerator = new Mock<INumGenerator>();
            moqGenerator.Setup(g => g.GenerateId()).Returns(fakeId);
            FakeNumGenerator = moqGenerator.Object;

            var moqRepository = new Mock<IRepository<User>>();
            moqRepository.Setup(r => r.Add(It.IsAny<User>()));
            FakeRepository = moqRepository.Object;
            FakeValidator = new EmptyUserValidator();
        }

        public User SimpleUser { get; set; } = new User
        {
            FirstName = "Ivan2",
            LastName = "Ivanov2",
            PersonalId = "MP12345",
            BirthDate = DateTime.Now,
        };

        private UserService Service { get; set; }

        private INumGenerator FakeNumGenerator { get; set; }

        private IRepository<User> FakeRepository { get; set; }

        private EmptyUserValidator FakeValidator { get; set; }

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

            Service = new MasterUserService(new EvenIdGenerator(), new EmptyUserValidator(), new UserRepository(null, null));
            int requiredId = Service.Add(requiredUser);
            Service.Add(anotherUser);

            var searchUserId = Service.SearchForUsers(new Func<User, bool>[]
            {
                u => u.LastName == "Ivanov" && u.PersonalId == "MP12345"
            }).FirstOrDefault();

            Assert.AreEqual(requiredId, searchUserId);
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

        [Test]
        public void ThreadSynchronization_Test()
        {
            Random random = new Random();
            var userRepository = new UserRepository(null, null);
            ValidatorBase<User> validator = new EmptyUserValidator();
            Service = new MasterUserService(new EvenIdGenerator(), validator, userRepository);
            int threadsCount = 5;
            int iterationCount = 3;
            IList<Thread> threads = new List<Thread>(threadsCount);
            var readThread = new Thread(() =>
            {
                int iterations = iterationCount;
                while (iterations-- > 0)
                {
                    var usersIds = Service.SearchForUsers(new Func<User, bool>[]
                    {
                        u => u.PersonalId != null
                    });
                    Console.WriteLine("Users ID: ");
                    foreach (var userId in usersIds)
                    {
                        Console.Write(userId + " ");
                    }

                    Console.WriteLine();
                    Thread.Sleep(2000);
                }
            });
            readThread.Start();
            for (int i = 0; i < threadsCount; i++)
            {
                var user = new User
                {
                    PersonalId = $"MP{i}"
                };
                var thread = new Thread(() =>
                {
                    int iterations = iterationCount;
                    while (iterations-- > 0)
                    {
                        Service.Add(user);
                        Console.WriteLine("User added " + user.PersonalId);
                        Thread.Sleep((int)(random.NextDouble() * 1000));
                        Service.Delete(user);
                        Console.WriteLine("User deleted " + user.PersonalId);
                        Thread.Sleep((int)(random.NextDouble() * 1000));
                    }
                });
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        [Test]
        public void SearchByCriteria_PersonalIdCriteria_ReturnedTwoUsers()
        {
            var service = new MasterUserService(FakeNumGenerator, FakeValidator, new UserRepository(null, null), false);
            int users = 100000;
            service.Add(SimpleUser);
            service.Add(new User { PersonalId = "PM321" });
            for (int i = 0; i < users; i++)
            {
                var user = new User { PersonalId = GenerateString() };
                service.Add(user);
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = service.SearchForUsers(new ICriteria<User>[] { new CriterionPersonalId() });
            Console.WriteLine(stopwatch.Elapsed);
        }

        private static string GenerateString()
        {
            Guid g = Guid.NewGuid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            guidString = guidString.Replace("=", string.Empty);
            guidString = guidString.Replace("+", string.Empty);
            return guidString;
        }
    }
}