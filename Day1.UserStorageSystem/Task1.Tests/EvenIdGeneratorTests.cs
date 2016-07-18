using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.IdGenerator;

namespace Task1.Tests
{
    [TestFixture]
    class EvenIdGeneratorTests
    {
        [Test]
        public void GenerateId_GetFirstFiveIDs_NumbersIsUnique()
        {
            int count = 5;
            EvenIdGenerator generator = new EvenIdGenerator();
            int[] IDs = new int[count];

            for (int i = 0; i < count; i++)
            {
                IDs[i] = generator.GenerateId();

                //temp 
                Debug.WriteLine(IDs[i]);
            }

            bool numbersAreUnique = IDs.Distinct().Count() == IDs.Length;

            Assert.IsTrue(numbersAreUnique);
        }

        [Test]
        public void GenerateId_LastGeneratedIdIsSix_NextGeneratedIdIsEight()
        {
            int lastGeneratedId = 6;
            int expectedId = 8;
            EvenIdGenerator generator = new EvenIdGenerator(lastGeneratedId);

            int id = generator.GenerateId();

            Assert.AreEqual(expectedId, id);
        }
    }
}
