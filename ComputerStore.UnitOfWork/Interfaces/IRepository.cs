//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.Structure.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ComputerStore.UnitOfWork.Interfaces
{
    /// <summary>
    /// Interface for generic repository, contains CRUD operation of EF entity
    /// </summary>
    /// <typeparam name="T">EF entity</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets the specified identifier. Asynchronous version.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <returns>Task Entity</returns>
        Task<T> GetAsync<TKey>(TKey id);       

        /// <summary>
        /// Gets the specified identifier include child entity
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TProperty">Child entity</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="navigationPropertyPath">Child property to include</param>
        /// <returns>Entity</returns>
        Task<T> GetAsync<TKey, TProperty>(TKey id, Expression<Func<T, TProperty>> navigationPropertyPath) where TProperty : class;

        /// <summary>
        /// Generic find by predicate
        /// </summary>
        /// <param name="predicate">Query predicate</param>
        /// <returns>Entity</returns>
        Task<T> FindByAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Generic find by predicate and option to include child entity
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include sub-entity.</param>
        /// <returns>First entity or default</returns>
        Task<T> FindByAsync(Expression<Func<T, bool>> predicate, string include);

        /// <summary>
        /// Generic find by predicate and option to include child entity
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="include">The include sub-entity.</param>
        /// <returns>First entity or default</returns>
        Task<T> FindByAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get all async include child property
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, string include);

        /// <summary>
        /// Gets all. With data pagination.
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <param name="pagingContext">The paging context.</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, PagingContext pagingContext);

        /// <summary>
        /// Gets all. With data pagination.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="pageCount">The page count.</param>
        /// <param name="navigationPropertyPath">Child entity to include</param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync<TProperty>(int page, int pageCount, Expression<Func<T, TProperty>> navigationPropertyPath);
        

        /// <summary>
        /// Gets all and offers to include a related table
        /// </summary>
        /// <param name="include">Te sub.entity to include</param>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync(string include);

        /// <summary>
        ///     <para>
        ///         Creates a LINQ query based on a raw SQL query.
        ///     </para>
        ///     <para>
        ///         If the database provider supports composing on the supplied SQL, you can compose on top of the raw SQL query using
        ///         LINQ operators - <code>context.Blogs.FromSql("SELECT * FROM dbo.Blogs").OrderBy(b =&gt; b.Name)</code>.
        ///     </para>
        ///     <para>
        ///         As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
        ///         attack. You can include parameter place holders in the SQL query string and then supply parameter values as additional
        ///         arguments. Any parameter values you supply will automatically be converted to a DbParameter -
        ///         <code>context.Blogs.FromSql("SELECT * FROM [dbo].[SearchBlogs]({0})", userSuppliedSearchTerm)</code>.
        ///     </para>
        ///     <para>
        ///         You can also construct a DbParameter and supply it to as a parameter value. This allows you to use named
        ///         parameters in the SQL query string -
        ///         <code>context.Blogs.FromSql("SELECT * FROM [dbo].[SearchBlogs]({@searchTerm})", new SqlParameter("@searchTerm", userSuppliedSearchTerm))</code>
        ///     </para>
        /// </summary>
        /// <param name="sql"> The raw SQL query. </param>
        /// <param name="parameters"> The values to be assigned to parameters. </param>
        /// <returns>List of entities</returns>
        IQueryable<T> FromSql(string sql, params object[] parameters);

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The Entity's state</returns>
        EntityState Add(T entity);

        /// <summary>
        /// Deletes the specified <paramref name="entity"/>
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        /// <returns>The Entity's state</returns>
        EntityState Delete(T entity);
        /// <summary>
        /// Checks whether a entity async matching the <paramref name="predicate"/> exists
        /// </summary>
        /// <param name="predicate">The predicate to filter on</param>
        /// <returns>Whether an entity matching the <paramref name="predicate"/> exists</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The Entity's state</returns>
        EntityState Update(T entity);

        /// <summary>
        /// Count total record after filter
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets all. With data pagination and include
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="pagingContext"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, PagingContext pagingContext,
            string[] includes);

        /// <summary>
        /// Get max value of selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<float> GetMaxAsync(Expression<Func<T, float>> selector);

        /// <summary>
        /// Get min value of selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<float> GetMinAsync(Expression<Func<T, float>> selector);
    }
}
