using System.Collections.Generic;

namespace Task1.StorageSystem.Entities
{
    public class SerializedUserData
    {
        public int LastGeneratedId { get; set; }
        public List<User> Users { get; set; } 
    }
}