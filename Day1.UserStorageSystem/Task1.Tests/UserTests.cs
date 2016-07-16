using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Task1.StorageSystem;
using Task1.StorageSystem.Entities;

namespace Task1.Tests
{
    [TestFixture]
    public class UserTests
    {
        public User TestUser { get; set; } = new User
        {
            PersonalId = "MP123456",
            FirstName = "Ivan",
            LastName = "Ivanov"
        };

        [Test]
        public void User_EqualsWithHimself_ReturnTrue()
        {
            Assert.IsTrue(TestUser.Equals(TestUser));
        }

        [Test]
        public void User_EqualsWithUserWithOtherPersonalId_ReturnFalse()
        {
            var someUser = new User
            {
                PersonalId = "MP45454",
                FirstName = "Ivan",
                LastName = "Ivanov"
            };
            Assert.IsFalse(TestUser.Equals(someUser));
        }

        [Test]
        public void User_EqualsWithEqualUser_ReturnTrue()
        {
            var equalUser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP123456"
            };
        }

        [Test]
        public void User_EqualsWithNull_ReturnFalse()
        {
            Assert.IsFalse(TestUser.Equals(null));
        }

        [Test]
        public void User_EqualsWithSimpleObject_ReturnFalse()
        {
            object someObject = new object();
            Assert.IsFalse(TestUser.Equals(someObject));
        }

        [Test]
        public void User_InvokeGetHashCodeTwiceOnTheSameUserAndCompare_HashCodesAreEqual()
        {
            Debug.WriteLine(TestUser.GetHashCode());
            Assert.AreEqual(TestUser.GetHashCode(), TestUser.GetHashCode());
        }

        [Test]
        public void User_CompareHashCodeWithOtherUser_ReturnFalse()
        {
            var otherUser = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                PersonalId = "MP3421"
            };

            Assert.AreNotEqual(TestUser.GetHashCode(), otherUser.GetHashCode());
        }

    }
}
