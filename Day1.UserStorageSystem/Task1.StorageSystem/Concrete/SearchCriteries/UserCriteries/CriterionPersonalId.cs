using System.Collections.Generic;
using System.Linq;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries
{
    public class CriterionPersonalId : ICriteria<User>
    {
        public IList<User> MeetCriteria(IList<User> users)
        {
            return users.Where(u => u.PersonalId != null).ToList();
        }
    }
}