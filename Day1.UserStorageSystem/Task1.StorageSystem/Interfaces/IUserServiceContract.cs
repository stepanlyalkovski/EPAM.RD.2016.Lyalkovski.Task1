namespace Task1.StorageSystem.Interfaces
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using Concrete.SearchCriteries.UserCriteries;
    using Entities;

    [ServiceContract]
    [ServiceKnownType(typeof(CriterionFemales))]
    [ServiceKnownType(typeof(CriterionMales))]
    [ServiceKnownType(typeof(CriterionPersonalId))]
    public interface IUserServiceContract
    {
        [OperationContract]
        int Add(User user);

        [OperationContract]
        void Delete(User user);

        [OperationContract]
        void Save();

        [OperationContract]
        void Initialize();

        [OperationContract]
        List<int> SearchForUsers(ICriteria<User>[] criteries);

    }
}