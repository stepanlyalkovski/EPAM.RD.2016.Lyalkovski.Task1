using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries
{
    public class CriterionMales : ICriteria<User>
    {
        public IList<User> MeetCriteria(IList<User> users)
        {
            return users.Where(u => u.Gender == Gender.Male).ToList();
        }
    }

    public class CriterionFemales : ICriteria<User>
    {
        public IList<User> MeetCriteria(IList<User> users)
        {
            return users.Where(u => u.Gender == Gender.Female).ToList();
        }
    }
}
