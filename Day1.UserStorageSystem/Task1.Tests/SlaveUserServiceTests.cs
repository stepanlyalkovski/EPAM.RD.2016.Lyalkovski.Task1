using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Moq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace Task1.Tests
{
    [TestFixture]
    public class SlaveUserServiceTests
    {
        public SlaveUserServiceTests()
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

        private INumGenerator FakeNumGenerator { get; set; }

        private IRepository<User> FakeRepository { get; set; }

        private EmptyUserValidator FakeValidator { get; set; }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Add_AddSimpleUser_ThrownedNotSupportedException()
        {
            var service = new SlaveUserService(FakeNumGenerator, FakeValidator, FakeRepository);
            service.Add(SimpleUser);
        }

        [Test]
        public void MasterServiceAdd_CreateSlaveAndSubscribeToMasterEditEvent_EventReceivedTwiceAfterAddAndDelete()
        {
            var userRepository = new UserRepository(null, null);
            var master = new MasterUserService(FakeNumGenerator, FakeValidator, userRepository);
            var slave = new SlaveUserService(FakeNumGenerator, FakeValidator, userRepository);
            slave.Subscribe(master);
            int eventsNumber = 2;
            int receivedEvents = 0;
            master.Deleted += delegate 
            {
                receivedEvents++;
            };
            master.Added += delegate 
            {
                receivedEvents++;
            };

            master.Add(SimpleUser);
            master.Delete(SimpleUser);

            Assert.AreEqual(eventsNumber, receivedEvents);
        }

        [Test]
        public void OnAdded_AddUserToMasterService_UserWasAddedAndExistsInSlaveService()
        {
            var masterRepository = new UserRepository(null, null);
            var slaveRepository = new UserRepository(null, null);
            var master = new MasterUserService(FakeNumGenerator, FakeValidator, masterRepository);
            var slave = new SlaveUserService(FakeNumGenerator, FakeValidator, slaveRepository);
            slave.Subscribe(master);
            int userIdFromMaster = master.Add(SimpleUser);
            int userIdFromSlave = slave.SearchForUsers(new Func<User, bool>[]
            {
                u => u.PersonalId == SimpleUser.PersonalId
            }).First();

            Assert.AreEqual(userIdFromMaster, userIdFromSlave);
        }

        [Test]
        public void OnDeleted_DeletedUserFromMasterService_UserWasDeletedAndDoesNotExistInSlaveService()
        {
            var masterRepository = new UserRepository(null, null);
            var slaveRepository = new UserRepository(null, null);
            var master = new MasterUserService(FakeNumGenerator, FakeValidator, masterRepository);
            var slave = new SlaveUserService(FakeNumGenerator, FakeValidator, slaveRepository);
            slave.Subscribe(master);
            int userIdFromMaster = master.Add(SimpleUser);

            master.Delete(SimpleUser);
            var searchResults = slave.SearchForUsers(new Func<User, bool>[]
            {
                u => u.PersonalId == SimpleUser.PersonalId
                      && u.Id == SimpleUser.Id
            }).ToList();

            Assert.IsEmpty(searchResults);
        }

        [Test]
        public void BolleanSwitch_Test()
        {
            var value = ConfigurationManager.AppSettings["test"];
            Debug.WriteLine(value);            
        }
    }
}