using System;
using System.Collections.Generic;
using System.ServiceModel;
using Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Interfaces
{
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