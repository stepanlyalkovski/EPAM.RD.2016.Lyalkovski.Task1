using System;
using System.Collections.Generic;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.IdGenerator
{
    public class EvenIdGenerator : INumGenerator
    {
        private readonly IEnumerator<int> _enumerator;

        public EvenIdGenerator(int lastGeneratedId = -1)
        {
            //NumberGenerator is expecting startPosition, so we have to increment our lastId
            int startPositionNumber = lastGeneratedId + 1;
            _enumerator = NumberGenerator.GetEvenNumbers(startPositionNumber).GetEnumerator();
        } 
        public int GenerateId()
        {
            if (_enumerator.MoveNext())
            {
                return _enumerator.Current;
            }
            
            throw new InvalidOperationException();
        }
    }
}
