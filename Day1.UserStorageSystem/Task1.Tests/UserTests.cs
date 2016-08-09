using System.Diagnostics;
using NUnit.Framework;
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
            Assert.IsTrue(this.TestUser.Equals(this.TestUser));
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
            Assert.IsFalse(this.TestUser.Equals(someUser));
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
            Assert.IsFalse(this.TestUser.Equals(null));
        }

        [Test]
        public void User_EqualsWithSimpleObject_ReturnFalse()
        {
            object someObject = new object();
            Assert.IsFalse(this.TestUser.Equals(someObject));
        }

        [Test]
        public void User_InvokeGetHashCodeTwiceOnTheSameUserAndCompare_HashCodesAreEqual()
        {
            Debug.WriteLine(this.TestUser.GetHashCode());
            Assert.AreEqual(this.TestUser.GetHashCode(), this.TestUser.GetHashCode());
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

            Assert.AreNotEqual(this.TestUser.GetHashCode(), otherUser.GetHashCode());
        }

    }
}
