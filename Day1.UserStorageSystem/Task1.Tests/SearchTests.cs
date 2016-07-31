using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Task1.StorageSystem.Concrete;
using Task1.StorageSystem.Concrete.SearchCriteries;
using Task1.StorageSystem.Concrete.SearchCriteries.UserCriteries;
using Task1.StorageSystem.Entities;
using Task1.StorageSystem.Interfaces;

namespace Task1.Tests
{
    [TestFixture]
    public class SearchTests
    {
        [Test]
        public void RepositorySearch_SearchMaleUsers_ReturnedAllMaleUsersInRepository()
        {
            int maleNumber = 100;
            var repository = new UserRepository(null, null);
            var maleUsers = Enumerable.Repeat(new User {Gender = Gender.Male}, maleNumber).ToList();
            var femaleUsers = Enumerable.Repeat(new User {Gender = Gender.Female}, maleNumber - 1);
            foreach (var user in maleUsers)
            {
                repository.Add(user);
            }

            ICriteria<User> maleCriteria = new CriterionMales();
            var result = repository.SearchByCriteria(maleCriteria);
            
            Assert.AreEqual(maleNumber, result.Count());
        }
        
    }
}