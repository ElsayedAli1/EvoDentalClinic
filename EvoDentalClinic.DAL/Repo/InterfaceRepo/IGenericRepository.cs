using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EvoDentalClinic.DAL.Repo.InterfaceRepo
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<(IReadOnlyList<T> Items, int Total)> GetPagedAsync(
    Expression<Func<T, bool>>? predicate,
    int page,
    int pageSize,
    Expression<Func<T, object>> orderBy,
    bool descending = false);
    }
}
