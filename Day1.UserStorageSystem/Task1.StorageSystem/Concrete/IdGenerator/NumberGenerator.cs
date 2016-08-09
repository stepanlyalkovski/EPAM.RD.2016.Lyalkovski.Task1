namespace Task1.StorageSystem.Concrete.IdGenerator
{
    using System.Collections.Generic;

    public static class NumberGenerator
    {
        public static IEnumerable<int> GetEvenNumbers(int startNumber = 0)
        {
            if (startNumber % 2 != 0)
            {
                startNumber++; // make it even
            }

            int previous = startNumber - 2;
            while (true)
            {
                if (startNumber > previous)
                {
                    yield return startNumber;
                }
                else
                {
                    yield break;
                }

                previous = startNumber;
                unchecked
                {
                    startNumber += 2;
                }
            }
        }
    }
}