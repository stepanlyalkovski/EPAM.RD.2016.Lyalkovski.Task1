using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;

namespace Task1.Tests
{
    [TestFixture]
    class SimpleUserValidatorTests
    {
        public SimpleUserValidator Validator { get; set; } = new SimpleUserValidator();
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Validate_LastNameIsNull_ThrownArgumentException()
        {
            var user = new User
            {
                FirstName = "Ivan",
                LastName = null
            };

            Validator.Validate(user);

        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Validate_FirstNameIsNull_ThrownArgumentException()
        {
            var user = new User
            {
                FirstName = null,
                LastName = "Ivanov"
            };

            Validator.Validate(user);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Validate_DateYearIsDefaultTypeValue_ThrownArgumentException()
        {
            var user = new User
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                BirthDate = new DateTime()
            };

            Validator.Validate(user);
        }
    }
}
