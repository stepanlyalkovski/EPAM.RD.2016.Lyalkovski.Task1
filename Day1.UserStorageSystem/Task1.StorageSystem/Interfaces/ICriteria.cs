namespace Task1.StorageSystem.Interfaces
{
    public interface ICriteria<T>
    {
        bool MeetCriteria(T entity);
    }
}