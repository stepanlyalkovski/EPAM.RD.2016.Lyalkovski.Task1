namespace Task1.StorageSystem.Interfaces
{
    using Entities;

    public interface IUserXmlFileWorker
    {
        void Save(SerializedUserData data, string filePath);

        SerializedUserData Load(string filePath);
    }
}
