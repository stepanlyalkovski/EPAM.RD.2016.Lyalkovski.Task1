using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Task1.StorageSystem.Interfaces
{
    public interface IRepository<TEntity>
    {
        TEntity GetById(int id);
        IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> predicate);
        void Add(TEntity entity);
        void Delete(TEntity entity);
    }
}