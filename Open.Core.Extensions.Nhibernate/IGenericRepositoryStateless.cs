using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Open.Core.Extensions.Nhibernate
{
    public interface IGenericRepositoryStateless<TEntity>  where TEntity : class, IEntity
    {
        TReturn? Transaction<TReturn>(Func<TReturn> execute);
        Task<TReturn?> Transaction<TReturn>(Func<Task<TReturn>> execute);
        Task Insert(TEntity ent);
        void Delete(int id);
        void Refresh(TEntity ent);
        Task RefreshAsync(TEntity ent);
        Task<TEntity> FindById(int id);
        Task<IEnumerable<TEntity>> Get();
        Task<IList<TEntity>> FindDescending(Expression<Func<TEntity, bool>> wherExpression);

        Task<IList<TEntity>> FindDescending(Expression<Func<TEntity, bool>> wherExpression,
            Expression<Func<TEntity, object>> orderByExpression);

        Task<IList<TEntity>> FindAscending(Expression<Func<TEntity, bool>> wherExpression);

        Task<IList<TEntity>> FindAscending(Expression<Func<TEntity, bool>> wherExpression,
            Expression<Func<TEntity, object>> orderByExpression);

        Task<IList<TEntity>> Find(
            Expression<System.Func<TEntity, bool>> wherExpression = default,
            Expression<System.Func<TEntity, object>> orderByExpression = default, bool asc = true);
    }
}