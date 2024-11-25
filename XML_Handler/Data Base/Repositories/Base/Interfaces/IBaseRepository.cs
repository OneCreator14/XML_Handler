using System.Linq.Expressions;

namespace XML_Handler.DB.Repositories.Base.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        #region Methods

        Task<T> AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteManyAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>>? filter = null,
                                          Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                          int? top = null,
                                          int? skip = null,
                                          bool? distinct = null,
                                          params string[] includeProperties);

        Task<IEnumerable<string>> GetColumn(
                Expression<Func<T, bool>>? filter = null,
                Expression<Func<T, string>>? selector = null,
                bool? distinct = null
            );

        #endregion
    }
}
