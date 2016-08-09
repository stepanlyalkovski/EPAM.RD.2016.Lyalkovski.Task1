namespace Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries
{
    using System;
    using Entities;
    using Interfaces;

    [Serializable]
    public class CriterionPersonalId : ICriteria<User>
    {
        public bool MeetCriteria(User entity)
        {
            return entity.PersonalId != null;
        }
    }
}