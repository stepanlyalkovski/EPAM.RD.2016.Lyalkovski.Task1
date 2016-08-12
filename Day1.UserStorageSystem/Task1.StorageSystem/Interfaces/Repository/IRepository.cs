namespace Task1.StorageSystem.Interfaces.Repository
{
    using System;
    using System.Collections.Generic;
    using Entities;

    public interface IRepository<TEntity>
    {
        IEnumerable<int> SearhByPredicate(Func<TEntity, bool>[] predicates);

        IEnumerable<int> SearchByCriteria(ICriteria<TEntity>[] criteries);

        void Add(TEntity user);

        void Save(int lastGeneratedId);

        IEnumerable<User> GetAll(); 

        void Initialize(); 

        int GetState(); // for lastGeneratedId

        TEntity GetById(int id);

        void Delete(TEntity entity);

        void Clear();
    }
}