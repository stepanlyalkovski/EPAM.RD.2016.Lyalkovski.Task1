using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Interfaces
{
    public interface ICriteria<T>
    {
        bool MeetCriteria(T entity);
    }
}