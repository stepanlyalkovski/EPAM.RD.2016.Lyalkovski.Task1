using System.Collections.Generic;
using Task1.StorageSystem.Concrete.Validation;
using Task1.StorageSystem.Entities;

namespace ServiceConfigurator.Tests
{
    public class EmptyUserValidator : ValidatorBase<User>
    {
        protected override IEnumerable<Rule> Rules => new List<Rule>();
    }
}