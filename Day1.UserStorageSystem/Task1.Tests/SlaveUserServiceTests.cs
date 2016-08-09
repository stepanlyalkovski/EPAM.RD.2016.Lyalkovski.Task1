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
        //Using Moq to create fake entities
        private INumGenerator FakeNumGenerator { get; set; }
        private IRepository<User> FakeRepository { get; set; }
        private EmptyUserValidator FakeValidator { get; set; }
        public User SimpleUser { get; set; } = new User
        {
            FirstName = "Ivan2",
            LastName = "Ivanov2",
            PersonalId = "MP12345",
            BirthDate = DateTime.Now,
        };
        public SlaveUserServiceTests()
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
        [ExpectedException(typeof(NotSupportedException))]
        public void Add_AddSimpleUser_ThrownedNotSupportedException()
        {
            var service = new SlaveUserService(this.FakeNumGenerator, this.FakeValidator, this.FakeRepository);
            service.Add(this.SimpleUser);
        }

        [Test]
        public void MasterServiceAdd_CreateSlaveAndSubscribeToMasterEditEvent_EventReceivedTwiceAfterAddAndDelete()
        {
            var userRepository = new UserRepository(null, null);
            var master = new MasterUserService(this.FakeNumGenerator, this.FakeValidator, userRepository);
            var slave = new SlaveUserService(this.FakeNumGenerator, this.FakeValidator, userRepository);
            slave.Subscribe(master);
            int eventsNumber = 2;
            int receivedEvents = 0;
            master.Deleted += delegate {
                receivedEvents++;
            };
            master.Added += delegate {
                receivedEvents++;
            };

            master.Add(this.SimpleUser);
            master.Delete(this.SimpleUser);

            Assert.AreEqual(eventsNumber, receivedEvents);
        }

        [Test]
        public void OnAdded_AddUserToMasterService_UserWasAddedAndExistsInSlaveService()
        {
            var masterRepository = new UserRepository(null, null);
            var slaveRepository = new UserRepository(null, null);
            var master = new MasterUserService(this.FakeNumGenerator, this.FakeValidator, masterRepository);
            var slave = new SlaveUserService(this.FakeNumGenerator, this.FakeValidator, slaveRepository);
            slave.Subscribe(master);
            int userIdFromMaster = master.Add(this.SimpleUser);
            int userIdFromSlave = slave.SearchForUsers(new Func<User, bool>[]
            {
                u => u.PersonalId == this.SimpleUser.PersonalId
            }).First();

            Assert.AreEqual(userIdFromMaster, userIdFromSlave);

        }

        [Test]
        public void OnDeleted_DeletedUserFromMasterService_UserWasDeletedAndDoesNotExistInSlaveService()
        {
            var masterRepository = new UserRepository(null, null);
            var slaveRepository = new UserRepository(null, null);
            var master = new MasterUserService(this.FakeNumGenerator, this.FakeValidator, masterRepository);
            var slave = new SlaveUserService(this.FakeNumGenerator, this.FakeValidator, slaveRepository);
            slave.Subscribe(master);
            int userIdFromMaster = master.Add(this.SimpleUser);

            master.Delete(this.SimpleUser);
            var searchResults = slave.SearchForUsers(new Func<User, bool>[]
            {
                u => u.PersonalId == this.SimpleUser.PersonalId
                      && u.Id == this.SimpleUser.Id
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