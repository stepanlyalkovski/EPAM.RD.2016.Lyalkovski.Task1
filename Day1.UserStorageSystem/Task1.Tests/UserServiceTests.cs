using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.IdGenerator;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using System.Linq;
using System.Threading;
using Castle.Core.Internal;
using Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries;
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
            var moqGenerator = new Mock<INumGenerator>();
            moqGenerator.Setup(g => g.GenerateId()).Returns(fakeId);
            this.FakeNumGenerator = moqGenerator.Object;

            var moqRepository = new Mock<IRepository<User>>();
            // stab for repository
            moqRepository.Setup(r => r.Add(It.IsAny<User>()));
            this.FakeRepository = moqRepository.Object;
            this.FakeValidator = new EmptyUserValidator();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_AddInvalidUser_ThrownArgumentExceptionWithValidationMessages()
        {
            ValidatorBase<User> validator = new SimpleUserValidator();
            this.Service = new MasterUserService(this.FakeNumGenerator, validator, this.FakeRepository);

            var invalidUser = new User
            {
                FirstName = "Ivan",
                LastName = null,
                BirthDate = new DateTime()
            };

            this.Service.Add(invalidUser);
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
            this.Service = new MasterUserService(this.FakeNumGenerator, validator, userMemoryRepository);

            int id = this.Service.Add(validUser);
            var recievedId = this.Service.SearchForUsers(new Func<User, bool>[]
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
            this.Service = new MasterUserService(numGenerator, validator, userMemoryRepository);
            var receivedUsers = new List<User>();

            for (int i = 0; i < userCount; i++)
            {
                var validUser = new User
                {
                    FirstName = "generated",
                    LastName = $"User {i + 1}",
                    BirthDate = DateTime.Now
                };

                int userId = this.Service.Add(validUser);
            }

            var storageUserCount = this.Service.SearchForUsers(new Func<User, bool>[]
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
            this.Service = new MasterUserService(this.FakeNumGenerator, validator, userMemoryRepository);
            var user = new User
            {
                FirstName = "Default",
                LastName = "Default",
                BirthDate = DateTime.Now
            };

            int userId = this.Service.Add(user);
            this.Service.Delete(user);
            var users = this.Service.SearchForUsers(new Func<User, bool>[] { u => u.Id == userId });

            Assert.IsEmpty(users);
        }

        [Test]
        public void Delete_SendNullToDelete_IgnoreWithNoExceptions()
        {
            var userMemoryRepository = new UserRepository(null, null);
            ValidatorBase<User> validator = new SimpleUserValidator();
            this.Service = new MasterUserService(this.FakeNumGenerator, validator, userMemoryRepository);
            this.Service.Delete(null);

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
            this.Service = new MasterUserService(new EvenIdGenerator(), new EmptyUserValidator(),
                                                new UserRepository(null, null));
            int requiredId = this.Service.Add(requiredUser);
            this.Service.Add(anotherUser);

            var searchUserId = this.Service.SearchForUsers(new Func<User, bool>[]
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
            this.Service = new MasterUserService(new EvenIdGenerator(lastId), validator, userMemoryRepository);
            this.Service.Add(this.SimpleUser);
            this.Service.Save();
            var anotheruser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP1",
                BirthDate = DateTime.Now,
            };

            this.Service.Initialize();
            int id = this.Service.Add(anotheruser);

            Debug.WriteLine(id);
            Assert.AreEqual(expectedId, id);
        }

        [Test]
        public void Initialize_SaveUserAndThanInitilizeRepository_ReturnedEqualUserId()
        {
            string filePath = "D://forTests.xml";
            var userMemoryRepository = new UserRepository(new UserXmlFileWorker(), filePath);
            ValidatorBase<User> validator = new SimpleUserValidator();
            this.Service = new MasterUserService(new EvenIdGenerator(), validator, userMemoryRepository);
            int expectedId = this.Service.Add(this.SimpleUser);
            this.Service.Save();
            this.Service.Initialize();
            var predicates = new Func<User, bool>[] { p => p.PersonalId == this.SimpleUser.PersonalId };
            var user = this.Service.SearchForUsers(predicates).First();
            Assert.AreEqual(expectedId, user);
        }

        // Trying to find way to test Deep Clone
        [Test]
        public void Add_EditAddedUser_UserInRepositoryWasNotEdited()
        {
            var userRepository = new UserRepository(null, null);
            ValidatorBase<User> validator = new SimpleUserValidator();
            this.Service = new MasterUserService(new EvenIdGenerator(), validator, userRepository);
            int id = this.Service.Add(this.SimpleUser);

            this.SimpleUser.Id = id + 1; // change User ID in local object

            int userRepositoryId = userRepository.SearhByPredicate(new Func<User, bool>[]
            {
                u => u.PersonalId == this.SimpleUser.PersonalId
            }).First();
            Debug.WriteLine("UserRepositoryId:" + userRepositoryId);
            Assert.AreNotEqual(this.SimpleUser.Id, userRepositoryId);
        }

        [Test]
        public void ThreadSynchronization_Test()
        {
            Random random = new Random();
            var userRepository = new UserRepository(null, null);
            ValidatorBase<User> validator = new EmptyUserValidator();
            this.Service = new MasterUserService(new EvenIdGenerator(), validator, userRepository);
            int threadsCount = 5;
            int iterationCount = 3;
            IList<Thread> threads = new List<Thread>(threadsCount);
            var readThread = new Thread(() =>
            {
                int iterations = iterationCount;
                while (iterations-- > 0)
                {
                    var usersIds = this.Service.SearchForUsers(new Func<User, bool>[]
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
                        this.Service.Add(user);
                        Console.WriteLine("User added " + user.PersonalId);
                        Thread.Sleep((int)(random.NextDouble() * 1000));
                        this.Service.Delete(user);
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
            var service = new MasterUserService(this.FakeNumGenerator, this.FakeValidator, new UserRepository(null, null), false);
            int users = 100000;
            service.Add(this.SimpleUser);
            service.Add(new User { PersonalId = "PM321" });
            for (int i = 0; i < users; i++)
            {
                var user = new User {PersonalId = GenerateString()};
                service.Add(user);
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            var result = service.SearchForUsers(new ICriteria<User>[] {new CriterionPersonalId()});
            Console.WriteLine(stopwatch.Elapsed);
        }

        private static string GenerateString()
        {
            Guid g = Guid.NewGuid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            guidString = guidString.Replace("=", "");
            guidString = guidString.Replace("+", "");
            return guidString;
        }
    }

}