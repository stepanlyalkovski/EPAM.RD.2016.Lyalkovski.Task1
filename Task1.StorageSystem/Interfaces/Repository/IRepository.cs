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
        int GetState();// for lastGeneratedId
        void Delete(TEntity entity);
    }
}