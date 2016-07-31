using System.Collections.Generic;

namespace Task1.StorageSystem.Interfaces
{
    public interface ICriteria<T>
    {
        IList<T> MeetCriteria(IList<T> entities);
    }
}