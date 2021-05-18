//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using ComputerStore.BoundedContext.Data;
using ComputerStore.UnitOfWork.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ComputerStore.UnitOfWork.Implement
{
    /// <summary>
    /// The Entity Framework implementation of IUnitOfWork
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The DbContext
        /// </summary>
        private IDbContext dbContext;

        /// <summary>
        /// The repositories
        /// </summary>
        private Dictionary<Type, object> _repositories;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="dbContext">The context factory.</param>
        public UnitOfWork(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class
        {
            if (this._repositories == null)
            {
                this._repositories = new Dictionary<Type, object>();
            }

            var type = typeof(TEntity);
            if (!this._repositories.ContainsKey(type))
            {
                this._repositories[type] = new Repository<TEntity>(this.dbContext);
            }

            return (IRepository<TEntity>)this._repositories[type];
        }

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        public int Commit()
        {
            // Save changes with the default options
            return this.dbContext.SaveChanges();
        }

        /// <summary>
        /// Asynchronous saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        public async Task<int> CommitAsync()
        {
            // Save changes with the default options
            return await this.dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(obj: this);
        }

        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (this.dbContext == null) return;
            this.dbContext.Dispose();
            this.dbContext = null;
        }
    }
}
