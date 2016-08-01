using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries
{
    [Serializable]
    public class CriterionMales : ICriteria<User>
    {
        public bool MeetCriteria(User user)
        {
            return user.Gender == Gender.Male;
        }
    }

    [Serializable]
    public class CriterionFemales : ICriteria<User>
    {
        public bool MeetCriteria(User user)
        {
            Console.WriteLine("FEMALE CRITERIA!");
            return user.Gender == Gender.Female;
        }
    }
}
