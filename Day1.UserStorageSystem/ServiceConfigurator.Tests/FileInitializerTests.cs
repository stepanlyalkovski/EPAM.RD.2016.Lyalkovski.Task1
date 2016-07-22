using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ServiceConfigurator.CustomSections.Files;

namespace ServiceConfigurator.Tests
{
    [TestFixture]
    public class FileInitializerTests
    {

        [Test]
        public void GetFilePath_Test()
        {
            string expectedFilePath = @"D:\test.xml";

            string filePath = FileInitializer.GetFilePath();
            
            Assert.AreEqual(expectedFilePath, filePath);
        }
       
    }
}
