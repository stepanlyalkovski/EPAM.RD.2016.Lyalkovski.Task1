using System;
using System.Linq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;

namespace Task1.Tests
{
    [TestFixture]
    public class SimpleUserValidatorTests
    {
        public SimpleUserValidator Validator { get; set; } = new SimpleUserValidator();
        [Test]
        public void Validate_LastNameIsNull_ReturnsProperErrorMessage()
        {
            var expectedMessage = "Last Name must not be null";
            var user = new User
            {
                FirstName = "Ivan",
                LastName = null
            };

            var messages = Validator.Validate(user);
            Assert.IsTrue(messages.Contains(expectedMessage));
        }

        [Test]
        public void Validate_FirstNameIsNull_ReturnsProperErrorMessage()
        {
            var expectedMessage = "First Name must not be null";

            var user = new User
            {
                FirstName = null,
                LastName = "Ivanov"
            };

            var messages = Validator.Validate(user);
            Assert.IsTrue(messages.Contains(expectedMessage));
        }

        [Test]
        public void Validate_DateYearIsDefaultTypeValue_ReturnsProperErrorMessage()
        {
            var expectedMessage = "Year value must be more than 1900";
            var user = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                BirthDate = new DateTime()
            };

            var messages = Validator.Validate(user);
            Assert.IsTrue(messages.Contains(expectedMessage));
        }

        [Test]
        public void Validate_ValidUser_NoErrorMessagesReturned()
        {
            var user = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                BirthDate = DateTime.Now
            };
            var messages = Validator.Validate(user);
            Assert.IsTrue(!messages.Any());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Validate_SendNull_ThrownArgumentNullException()
        {
            Validator.Validate(null);
        }
    }
}
