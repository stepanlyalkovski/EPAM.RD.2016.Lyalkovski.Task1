using System;
using System.Collections.Generic;
using Task1.StorageSystem.Entities;

namespace Task1.StorageSystem.Concrete.Validation
{
    public class SimpleUserValidator : ValidatorBase<User>
    {
        protected override IEnumerable<Rule> Rules
        {
            get
            {
                return new Rule[] 
                {
                    new Rule { Test = u => u.LastName != null, Message = "Last Name must not be null" },
                    new Rule { Test = u => u.FirstName != null, Message = "First Name must not be null" },
                    new Rule {Test = u => u.BirthDate.Year > 1900, Message = "Year value must be more than 1900"}
                };
            }
        }
    }
}