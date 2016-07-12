using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete
{
    public class EvenIdGenerator : INumGenerator
    {
        private readonly IEnumerator<int> _enumerator;

        public EvenIdGenerator()
        {
            _enumerator = NumberGenerator.GetEvenNumbers().GetEnumerator();
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
