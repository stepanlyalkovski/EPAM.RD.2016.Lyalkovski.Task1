namespace Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries
{
    using System;
    using Entities;
    using Interfaces;

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