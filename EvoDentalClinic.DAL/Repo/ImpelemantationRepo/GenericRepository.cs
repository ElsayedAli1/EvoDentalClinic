using EvoDentalClinic.DAL.Context;
using EvoDentalClinic.DAL.Repo.InterfaceRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EvoDentalClinic.DAL.Repo.ImpelemantationRepo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync(
            params Expression<Func<T, object>>[] includes)
            => await ApplyIncludes(_dbSet, includes).ToListAsync();

        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
            => await ApplyIncludes(_dbSet.Where(predicate), includes).ToListAsync();

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Remove(T entity)
            => _dbSet.Remove(entity);

        private static IQueryable<T> ApplyIncludes(
            IQueryable<T> query, Expression<Func<T, object>>[] includes)
        {
            foreach (var include in includes)
                query = query.Include(include);

            return query;
        }

        public async Task<(IReadOnlyList<T> Items, int Total)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate,
        int page,
        int pageSize,
        Expression<Func<T, object>> orderBy,
        bool descending = false)
        {
            IQueryable<T> query = _dbSet;

            if (predicate is not null)
                query = query.Where(predicate);

            var total = await query.CountAsync();

            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

    }
}
