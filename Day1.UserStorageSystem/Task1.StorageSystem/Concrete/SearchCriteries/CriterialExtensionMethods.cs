using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.SearchCriteries
{
    public static class CriterialExtensionMethods
    {
        public static ICriteria<T> And<T>(this ICriteria<T> criteria, ICriteria<T> otherCriteria)
        {
            return new AndCriteria<T>(criteria, otherCriteria);
        }

        public static ICriteria<T> Or<T>(this ICriteria<T> criteria, ICriteria<T> otherCriteria)
        {
            return new OrCriteria<T>(criteria, otherCriteria);
        }
    }
}