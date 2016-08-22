using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete.IdGenerator;

namespace Task1.Tests
{
    [TestFixture]
    public class EvenIdGeneratorTests
    {
        [Test]
        public void GenerateId_GetFirstFiveIDs_NumbersIsUnique()
        {
            int count = 5;
            EvenIdGenerator generator = new EvenIdGenerator();
            int[] ds = new int[count];

            for (int i = 0; i < count; i++)
            {
                ds[i] = generator.GenerateId();
                Debug.WriteLine(ds[i]);
            }

            bool numbersAreUnique = ds.Distinct().Count() == ds.Length;

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
