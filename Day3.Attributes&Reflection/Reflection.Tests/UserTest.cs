using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Attributes;
using Attributes.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reflection.Tests
{


    [TestClass]
    public class UserTest
    {
        private static bool Equal(Attributes.User user1, Attributes.User user2)
        {
            return user1.Id == user2.Id
                   && user1.FirstName == user2.FirstName
                   && user1.LastName == user2.LastName;
        }
        //private class UserComparer
        [TestMethod]
        public void InstanceAttribute_ThreeAttributes_ThreeObjects()
        {
            int count = 3;
            var users = UserReflection.CreateInstances();
            Assert.AreEqual(count, users.Count());
        }

        [TestMethod]
        public void InstanceAttribute_ThreeAttributes_EqualToRequiredUsers()
        {
                //[InstantiateUser("Alexander", "Alexandrov")]
                //[InstantiateUser(2, "Semen", "Semenov")]
                //[InstantiateUser(3, "Petr", "Petrov")]
            int defaultValue = 1;
             IEnumerable<Attributes.User> users = new List<Attributes.User>
             {
                 new Attributes.User(defaultValue) {FirstName = "Alexander" , LastName = "Alexandrov" },
                 new Attributes.User(2) {FirstName = "Semen" , LastName = "Semenov" },
                 new Attributes.User(3) {FirstName = "Petr" , LastName = "Petrov" }
             };
            
            var resultUsers = UserReflection.CreateInstances().ToList();

            foreach (var resultUser in resultUsers)
            {
                Debug.WriteLine(resultUser.FirstName + " " + resultUser.LastName + " " + resultUser.Id);
            }

            // assert with custom comparer
        }

    }
}
