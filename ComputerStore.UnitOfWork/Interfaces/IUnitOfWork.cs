//-----------------------------------------------------------------------
// <copyright file="CategoryController.cs" company="Young">
//     Company copyright tag.
// </copyright>
// <author>ToanHD2</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace ComputerStore.UnitOfWork.Interfaces
{
    /// <summary>
    /// Contains functions to manipulate EF transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>Repository</returns>
        IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        int Commit();

        /// <summary>
        /// Asynchronous saves all pending changes
        /// </summary>
        /// <returns>The number of objects in an Added, Modified, or Deleted state</returns>
        Task<int> CommitAsync();
    }
}
