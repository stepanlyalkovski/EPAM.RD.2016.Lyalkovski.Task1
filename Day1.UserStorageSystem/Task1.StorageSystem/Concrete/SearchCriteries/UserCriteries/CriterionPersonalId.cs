using System;
using System.Collections.Generic;
using System.Linq;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries
{
    [Serializable]
    public class CriterionPersonalId : ICriteria<User>
    {
        public bool MeetCriteria(User entity)
        {
            return entity.PersonalId != null;
        }
    }
}