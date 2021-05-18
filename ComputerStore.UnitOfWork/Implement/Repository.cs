//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Data;
using ComputerStore.Structure.Extensions;
using ComputerStore.Structure.Models.Pagination;
using ComputerStore.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.UnitOfWork.Implement
{
    /// <summary>
    /// Generic repository, contains CRUD operation of EF entity
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class Repository<T> : IRepository<T>
        where T : class
    {
        /// <summary>
        /// EF data base context
        /// </summary>
        private readonly IDbContext context;

        /// <summary>
        /// Used to query and save instances of
        /// </summary>
        private readonly DbSet<T> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public Repository(IDbContext context)
        {
            this.context = context;            
            dbSet = context.Set<T>();
        }

        /// <summary>
        /// Gets the specified identifier by key. Asynchronous version.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>Task Entity</returns>
        public async Task<T> GetAsync<TKey>(TKey id)
        {
            return await dbSet.FindAsync(id);
        }

        /// <summary>
        /// Gets the specified identifier by key include related entity. Asynchronous version.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="navigationPropertyPath"></param>
        /// <returns>Task Entity</returns>
        public async Task<T> GetAsync<TKey, TProperty>(TKey id, Expression<Func<T, TProperty>> navigationPropertyPath) where TProperty : class
        {
            var entity = await dbSet.FindAsync(id);
            if (entity == null) return null;
            await context.Entry(entity).Reference(navigationPropertyPath).LoadAsync();
            return entity;
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>List of entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Gets all and include related entity.
        /// </summary>
        /// <returns>List of entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string include)
        {
            return await dbSet.Where(predicate).Include(include).ToListAsync();
        }

        /// <summary>
        /// Generic find by predicate
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>First entity or default</returns>
        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Generic find by predicate and option to include child entity
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include sub-entity.</param>
        /// <returns>First entity or default</returns>
        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate, string include)
        {
            return await dbSet.Where(predicate).Include(include).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns>List of entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        /// <summary>
        /// Gets all. With data pagination.
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="pagingContext">The paging context.</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, PagingContext pagingContext)
        {
            var pageSize = (pagingContext.PageNumber - 1) * pagingContext.NumberPerPage;
            var propertyInfos = typeof(T).GetProperties();

            if (propertyInfos.Any(x => x.Name == pagingContext.SortColums))
            {
                return await dbSet.Where(predicate)
                    .Sort(pagingContext.SortColums, pagingContext.SortDirection)
                    .Skip(pageSize).Take(pagingContext.NumberPerPage).ToListAsync();
            }
            return await dbSet.Where(predicate).Skip(pageSize).Take(pagingContext.NumberPerPage).ToListAsync();
        }

        /// <summary>
        /// Count total record after filter
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).CountAsync();
        }

        /// <summary>
        /// Gets all. With data pagination.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageCount">The page count.</param>
        /// <param name="navigationPropertyPath">Child entity to include</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync<TProperty>(int page, int pageCount, Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            var pageSize = (page - 1) * pageCount;

            return await dbSet.Include(navigationPropertyPath)
                .Skip(pageSize).Take(pageCount).ToListAsync();
        }

        /// <summary>
        /// Gets all and offers to include a related table
        /// </summary>
        /// <param name="include">Te sub.entity to include</param>
        /// <returns>List of entities</returns>
        public async Task<IEnumerable<T>> GetAllAsync(string include)
        {
            return await dbSet.Include(include).ToListAsync();
        }
        
        /// <summary>
        /// Get all async include child property
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include)
        {
            var query = dbSet.Where(predicate);
            return await include(query).ToListAsync();
        }

        /// <summary>
        /// Creates a LINQ query based on a raw SQL query.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IQueryable<T> FromSql(string query, params object[] parameters)
        {
            return dbSet.FromSqlRaw(query, parameters);
        }

        /// <summary>
        /// Checks whether a entity async matching the <paramref name="predicate"/> exists
        /// </summary>
        /// <param name="predicate">The predicate to filter on</param>
        /// <returns>Whether an entity matching the <paramref name="predicate"/> exists</returns>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Execute sql raw query
        /// </summary>
        /// <param name="sqlQuery"></param>
        /// <returns></returns>
        public IQueryable<T> FromSql(string sqlQuery)
        {
            return dbSet.FromSqlRaw(sqlQuery);
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The Entity's state</returns>
        public virtual EntityState Add(T entity)
        {
            return dbSet.Add(entity).State;
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity"/>
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <returns>The Entity's state</returns>
        public EntityState Delete(T entity)
        {
            return dbSet.Remove(entity).State;
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The Entity's state</returns>
        public virtual EntityState Update(T entity)
        {
            return dbSet.Update(entity).State;
        }

        /// <summary>
        /// Generic find by predicate and option to include child entity
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include sub-entity.</param>
        /// <returns>First entity or default</returns>
        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include)
        {
            var query = dbSet.Where(predicate);
            return await include(query).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets all. With data pagination and include
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pagingContext"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, PagingContext pagingContext, string[] includes)
        {
            var query = dbSet.Where(predicate);

            var propertyNames = typeof(T).GetProperties().Select(x => x.Name);

            if (includes != null && includes.Length > 0)
            {
                var validIncludes = includes.Where(x => propertyNames.Any(p => p.Equals(x)));

                query = validIncludes.Aggregate(query, (current, include) => current.Include(include));
            }

            var pageSize = (pagingContext.PageNumber - 1) * pagingContext.NumberPerPage;

            if (propertyNames.Any(x => x == pagingContext.SortColums))
            {
                return await query.Sort(pagingContext.SortColums, pagingContext.SortDirection)
                    .Skip(pageSize).Take(pagingContext.NumberPerPage).ToListAsync();
            }
            
            return await query.Skip(pageSize).Take(pagingContext.NumberPerPage).ToListAsync();
        }

        /// <summary>
        /// Get max value of selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<float> GetMaxAsync(Expression<Func<T, float>> selector)
        {
            return await dbSet.MaxAsync(selector);
        }

        /// <summary>
        /// Get min value of selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public async Task<float> GetMinAsync(Expression<Func<T, float>> selector)
        {
            return await dbSet.MinAsync(selector);
        }
    }
}
