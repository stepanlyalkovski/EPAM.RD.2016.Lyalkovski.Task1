using System.Collections.Generic;

namespace Task1.StorageSystem.Concrete
{
    public class NumberGenerator
    {
        public static IEnumerable<int> GetEvenNumbers()
        {
            int number = 0;
            int previous = 0;
            while (true)
            {
                if (number > previous)
                {
                    yield return number;
                }
                else
                {
                    yield break;
                }

                previous = number;
                unchecked
                {
                    number += 2;
                }
                
            }

        }
    }
}