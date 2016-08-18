namespace Task1.StorageSystem.Entities
{
    using System.Collections.Generic;

    public class SerializedUserData
    {
        public int LastGeneratedId { get; set; }

        public List<User> Users { get; set; } 
    }
}