using System.Collections.Generic;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.SearchCriteries
{
    public class OrCriteria<T> : ICriteria<T>
    {
        private ICriteria<T> _criteria;
        private ICriteria<T> _otherCriteria;

        public OrCriteria(ICriteria<T> criteria, ICriteria<T> otherCriteria)
        {
            _criteria = criteria;
            _otherCriteria = otherCriteria;
        }

        public IList<T> MeetCriteria(IList<T> entities)
        {
            IList<T> firstCriteriaItems = _criteria.MeetCriteria(entities);
            IList<T> otherCriteraItems = _otherCriteria.MeetCriteria(entities);

            foreach (T otherCriteraItem in otherCriteraItems)
            {
                if (!firstCriteriaItems.Contains(otherCriteraItem))
                    firstCriteriaItems.Add(otherCriteraItem);
            }

            return firstCriteriaItems;
        }
    }
}