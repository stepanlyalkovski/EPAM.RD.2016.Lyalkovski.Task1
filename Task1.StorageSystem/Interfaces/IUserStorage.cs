using System;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Interfaces
{
    public interface IUserStorage
    {
        int Add(User user);
        void Delete(User user);

        //User SearchForUser(Func<> )
    }
}