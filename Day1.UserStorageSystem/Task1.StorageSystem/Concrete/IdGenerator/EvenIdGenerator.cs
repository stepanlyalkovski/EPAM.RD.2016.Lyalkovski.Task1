namespace Task1.StorageSystem.Concrete.IdGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Interfaces;

    [Serializable]
    public class EvenIdGenerator : MarshalByRefObject, INumGenerator, ISerializable
    {
        private IEnumerator<int> enumerator;

        public EvenIdGenerator(int lastGeneratedId = -1)
        {
            // NumberGenerator is expecting startPosition, so we have to increment our lastId        
            int startPositionNumber = lastGeneratedId + 1;
            this.enumerator = NumberGenerator.GetEvenNumbers(startPositionNumber).GetEnumerator();
        }

        public EvenIdGenerator(SerializationInfo info, StreamingContext context)
        {
            int lastId = (int)info.GetValue("lastId", typeof(int));
            this.enumerator = NumberGenerator.GetEvenNumbers(lastId).GetEnumerator();
        }

        public int GenerateId()
        {
            if (this.enumerator.MoveNext())
            {
                return this.enumerator.Current;
            }
            
            throw new InvalidOperationException();
        }

        public void Initialize(int number)
        {
            // NumberGenerator is expecting startPosition, so we have to increment our lastId
            int startPositionNumber = number + 1;
            this.enumerator = NumberGenerator.GetEvenNumbers(startPositionNumber).GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("lastId", this.enumerator.Current, typeof(int));      
        }
    }
}
