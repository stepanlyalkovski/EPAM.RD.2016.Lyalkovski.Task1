using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.IdGenerator
{
    [Serializable]
    public class EvenIdGenerator : MarshalByRefObject, INumGenerator, ISerializable
    {
        private readonly IEnumerator<int> _enumerator;

        public EvenIdGenerator(int lastGeneratedId = -1)
        {
            //NumberGenerator is expecting startPosition, so we have to increment our lastId
            int startPositionNumber = lastGeneratedId + 1;
            _enumerator = NumberGenerator.GetEvenNumbers(startPositionNumber).GetEnumerator();
        }

        public EvenIdGenerator(SerializationInfo info, StreamingContext context)
        {
            int lastId = (int)info.GetValue("lastId", typeof(int));
            _enumerator = NumberGenerator.GetEvenNumbers(lastId).GetEnumerator();
        }

        public int GenerateId()
        {
            if (_enumerator.MoveNext())
            {
                return _enumerator.Current;
            }
            
            throw new InvalidOperationException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("lastId", _enumerator.Current, typeof(int));

        }
    }
}
