namespace Task1.StorageSystem.Interfaces
{
    public interface INumGenerator
    {
        int GenerateId();

        void Initialize(int number);
    }
}