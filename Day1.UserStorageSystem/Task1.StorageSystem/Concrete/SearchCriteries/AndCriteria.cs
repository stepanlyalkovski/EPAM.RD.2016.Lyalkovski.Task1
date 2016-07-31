using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.SearchCriteries
{
    public class AndCriteria<T> : ICriteria<T>
    {
        private ICriteria<T> _criteria;
        private ICriteria<T> _otherCriteria;

        public AndCriteria(ICriteria<T> criteria, ICriteria<T> otherCriteria)
        {
            _criteria = criteria;
            _otherCriteria = otherCriteria;
        }

        public IList<T> MeetCriteria(IList<T> entities)
        {
            var result = _criteria.MeetCriteria(entities);

            if (result.Count == 0)
                return result;

            return _otherCriteria.MeetCriteria(result);
        }
    }
}
