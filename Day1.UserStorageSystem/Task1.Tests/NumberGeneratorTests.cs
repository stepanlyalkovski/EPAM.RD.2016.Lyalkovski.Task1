using System.Linq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete.IdGenerator;

namespace Task1.Tests
{
    [TestFixture]
    public class NumGeneratorTests
    {
        [Test]
        public void GetNumbers_InvokeMethod_FirstNumberIsZero()
        {
            var numbers = NumberGenerator.GetEvenNumbers();
            Assert.AreEqual(0, numbers.First());
        }

        [Test]
        public void GetNumbers_GetFirstFiveNumbers_ReturnEvenNumberSequence()
        {
            int count = 5;
            var numbers = NumberGenerator.GetEvenNumbers().Take(count).ToList();
            Assert.IsNotNull(numbers);

            int[] evenNumbers = { 0, 2, 4, 6, 8 };
            for (int i = 0; i < count; i++)
            {
                Assert.AreEqual(evenNumbers[i], numbers[i]);
            }
        }

        [Test]
        public void GetNumbers_InvokeMethod_FirstHundredNubmersAreEven()
        {
            int count = 100;
            var numbers = NumberGenerator.GetEvenNumbers().Take(count).ToList();
            Assert.IsNotNull(numbers);
            foreach (var number in numbers)
            {
                bool isEven = number % 2 == 0;
                Assert.IsTrue(isEven);
            }
        }

        [Test]
        public void GetNumbers_InvokeMethodTwiceAndGetFirstFiveNumbers_EqualNumberSequence()
        {
            int count = 5;
            var numbers = NumberGenerator.GetEvenNumbers().Take(count).ToList();
            var anotherNumbers = NumberGenerator.GetEvenNumbers().Take(count).ToList();
            bool areEqual = numbers.SequenceEqual(anotherNumbers);
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GetNumbers_InvokeMethod_EverySequenceNumberGreaterThanPrevious()
        {
            int cout = 50;
            var numbers = NumberGenerator.GetEvenNumbers().Take(cout);
        }

        [Test]
        public void GetNumbers_StartNumberIsSix_ReturnSixAsFirstNumber()
        {
            int startNumber = 6;

            var numbers = NumberGenerator.GetEvenNumbers(startNumber);

            Assert.AreEqual(startNumber, numbers.First());
        }

        [Test]
        public void GetNumbers_StartNumberIsNine_ReturnTenAsFirstNumber()
        {
            int startNumber = 9;
            int expectedNumber = 10;
            var numbers = NumberGenerator.GetEvenNumbers(startNumber);

            Assert.AreEqual(expectedNumber, numbers.First());
        }
    }
}
