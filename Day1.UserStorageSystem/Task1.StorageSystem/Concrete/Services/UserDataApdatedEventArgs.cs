namespace Task1.StorageSystem.Concrete.Services
{
    using System;
    using Entities;

    [Serializable]
    public class UserDataApdatedEventArgs : EventArgs
    {
        public User User { get; set; }
    }
}
