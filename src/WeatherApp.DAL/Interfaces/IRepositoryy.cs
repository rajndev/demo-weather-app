using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WeatherApp.DAL.Interfaces
{
    public interface IRepositoryy<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        Task<TEntity> GetByID(object id);

        Task Insert(TEntity entity);

        void Update(TEntity entityToUpdate);

        Task Delete(object id);

        void Delete(TEntity entityToDelete);
    }
}