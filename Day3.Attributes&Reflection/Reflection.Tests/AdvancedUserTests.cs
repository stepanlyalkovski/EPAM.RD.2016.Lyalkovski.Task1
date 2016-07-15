using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Reflection.Tests
{
    [TestClass]
    public class AdvancedUserTests
    {
        [TestMethod]
        public void CreateAdvancedUser_AssemblyAttribute_UserPropertiesEqualToRequiredInAssemblyAttribute()
        {
            //[assembly: InstantiateAdvancedUser(4, "Pavel", "Pavlov", 2329423)]
            var expectedUser = new AdvancedUser(4, 2329423)
            {
                FirstName = "Pavel",
                LastName = "Pavlov"
            };

            var user = User.UserReflection.CreateAdvancedUser();

            Assert.AreEqual(expectedUser, user);
        }
    }
}
