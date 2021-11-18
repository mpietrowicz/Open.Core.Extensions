using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NHibernate;

namespace Open.Core.Extensions.Nhibernate
{
    public class GenericRepositoryStateless<TEntity> : IGenericRepositoryStateless<TEntity> where TEntity : class, IEntity
    {
        private IStatelessSession Session { get; }

        public GenericRepositoryStateless(IStatelessSession session)
        {
            Session = session;
        }
        
        public TReturn? Transaction<TReturn>(Func<TReturn> execute)
        {
            using var transaction = Session.BeginTransaction();
            try
            {
                var ob = execute();
                transaction.Commit();

                return ob;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task<TReturn?> Transaction<TReturn>(Func<Task<TReturn>> execute)
        {
            using var transaction = Session.BeginTransaction() ?? throw new ArgumentNullException("Session.BeginTransaction()");
            try
            {
                var ob = await execute();
                transaction.Commit();

                return ob;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

      

        public async Task Insert(TEntity ent)
        {
            await Session.InsertAsync(ent);
        }

        public void Delete(int id)
        {
            var o = FindById(id).GetAwaiter().GetResult();
            if (o != null)
            {
                Session.Delete(o);
            }
        }

        public void Refresh(TEntity ent)
        {
            Session.Refresh(ent);
        }
        public async Task RefreshAsync(TEntity ent)
        {
            await Session.RefreshAsync(ent);
        }
        public async Task<TEntity> FindById(int id)
        {
            return await Session.QueryOver<TEntity>().Where(x => x.Id == id).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<TEntity>> Get()
        {
            return await Session.QueryOver<TEntity>().ListAsync();
        }

        public async Task<IList<TEntity>> FindDescending(Expression<Func<TEntity, bool>> wherExpression)
        {
            return await Find(wherExpression, asc: false);
        }

        public async Task<IList<TEntity>> FindAscending(Expression<Func<TEntity, bool>> wherExpression)
        {
            return await Find(wherExpression, asc: true);
        }

        public async Task<IList<TEntity>> FindDescending(Expression<Func<TEntity, bool>> wherExpression,
            Expression<Func<TEntity, object>> orderByExpression)
        {
            return await Find(wherExpression, orderByExpression, false);
        }

        public async Task<IList<TEntity>> FindAscending(Expression<Func<TEntity, bool>> wherExpression,
            Expression<Func<TEntity, object>> orderByExpression)
        {
            return await Find(wherExpression, orderByExpression, true);
        }

        public async Task<IList<TEntity>> Find(
            Expression<System.Func<TEntity, bool>> wherExpression = default,
            Expression<System.Func<TEntity, object>> orderByExpression = default, bool asc = true)
        {
            Expression<System.Func<TEntity, bool>> exp = wherExpression;
            Expression<System.Func<TEntity, object>> orderExpr = orderByExpression;
            var query = Session.QueryOver<TEntity>();
            if (wherExpression != default)
            {
                query = query.Where(exp);
            }

            if (orderByExpression != default && asc)
            {
                return await query.OrderBy(orderExpr).Asc.ListAsync<TEntity>();
            }

            if (orderByExpression != default && !asc)
            {
                return await query.OrderBy(orderExpr).Desc.ListAsync<TEntity>();
            }


            return await query.ListAsync();
        }
    }
}