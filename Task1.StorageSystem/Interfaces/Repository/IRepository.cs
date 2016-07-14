using System;
using System.Collections.Generic;

namespace Task1.StorageSystem.Interfaces.Repository
{
    public interface IRepository<TEntity>
    {
        IEnumerable<int> SearhByPredicate(Func<TEntity, bool>[] predicates);
        void Add(TEntity user);
        void Save(int lastGeneratedId);
        void Initialize();
        void Delete(TEntity entity);
    }
}