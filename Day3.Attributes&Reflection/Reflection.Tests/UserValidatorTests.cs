using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reflection.User;

namespace Reflection.Tests
{
    [TestClass]
    public class UserValidatorTests
    {
        [TestMethod]
        public void ValidateUser_AddValidUser_ReturnedTrue()
        {
            int validID = 10;
            IEnumerable<Attributes.User> users = UserReflection.CreateUserInstances();
            var validUser = users.First();

            bool isValid = UserValidation.ValidateUser(validUser);

            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public void ValidateUser_AddInUserWithInvalidId_ReturnFalse()
        {
            int invalidId = -999;
            Attributes.User user = new Attributes.User(invalidId)
            {
                FirstName = "DefaultName",
                LastName = "DefaultLastName"
            };

            bool isValid = UserValidation.ValidateUser(user);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateUser_AddUserWithInvalidLastName_ReturnFalse()
        {
            int validID = 10;
            int invalidCharNumber = 100;
            string invalidString = String.Concat(Enumerable.Repeat(' ', invalidCharNumber));

            IEnumerable<Attributes.User> users = UserReflection.CreateUserInstances();
            var invalidUser = users.First();
            invalidUser.LastName = invalidString;

            bool isValid = UserValidation.ValidateUser(invalidUser);

            Assert.IsFalse(isValid);
        }

        [TestMethod]
        public void ValidateUser_AddValidAdvancedUser_ReturnTrue()
        {
            var advancedUser = UserReflection.CreateAdvancedUser();

            bool isValid = UserValidation.ValidateUser(advancedUser);

            Assert.IsTrue(isValid);
        }

    }
}