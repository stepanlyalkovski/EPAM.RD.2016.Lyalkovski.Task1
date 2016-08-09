using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete.Services;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;
using Task1.StorageSystem.Interfaces.Repository;

namespace ServiceConfigurator.Tests
{
    public class EmptyUserValidator : ValidatorBase<User>
    {
        protected override IEnumerable<Rule> Rules => new List<Rule>();
    }

    [TestFixture]
    public class ThreadInitializerTests
    {
        private INumGenerator FakeNumGenerator { get; set; }
        private IRepository<User> FakeRepository { get; set; }
        private EmptyUserValidator FakeValidator { get; set; }
        public ThreadInitializerTests()
        {
            int fakeId = 1;
            var moqGenerator = new Mock<INumGenerator>();
            moqGenerator.Setup(g => g.GenerateId()).Returns(fakeId);
            this.FakeNumGenerator = moqGenerator.Object;

            var moqRepository = new Mock<IRepository<User>>();
            // stab for repository
            moqRepository.Setup(r => r.Add(It.IsAny<User>()));
            moqRepository.Setup(r => r.Delete(It.IsAny<User>()));
            this.FakeRepository = moqRepository.Object;
            this.FakeValidator = new EmptyUserValidator();
        }
        [Test]
        public void InitializeThreads_AddOneMasterAndTwoSlaves_ReturnedThreeThreads()
        {
            int threadsCount = 3;
            var slave = new SlaveUserService(this.FakeNumGenerator, this.FakeValidator, this.FakeRepository) {Name = "slave1"};
            var slave2 = new SlaveUserService(this.FakeNumGenerator, this.FakeValidator, this.FakeRepository) {Name = "slave2"};
            var master = new MasterUserService(this.FakeNumGenerator, this.FakeValidator, this.FakeRepository) {Name = "master"};
            var threads = ThreadInitializer.InitializeThreads(master, new[] {slave, slave2});
            Thread.Sleep(5000);
            Assert.AreEqual(threadsCount, threads.Count());
        }
    }
}