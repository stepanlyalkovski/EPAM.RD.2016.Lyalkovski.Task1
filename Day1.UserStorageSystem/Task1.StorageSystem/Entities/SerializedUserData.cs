namespace Task1.StorageSystem.Entities
{
    using System.Collections.Generic;
    /// <summary>
    /// Represent serializable user service data with state.
    /// </summary>
    public class SerializedUserData
    {
        public int LastGeneratedId { get; set; }

        public List<User> Users { get; set; } 
    }
}