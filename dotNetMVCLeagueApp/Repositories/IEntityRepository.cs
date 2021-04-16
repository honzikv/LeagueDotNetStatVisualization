using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models;

namespace dotNetMVCLeagueApp.Repositories {
    /// <summary>
    /// Abstraction layer between controller and EF Core for more concise code
    /// </summary>
    /// <typeparam name="T">Entity class</typeparam>
    public interface IEntityRepository<T> where T : class {
        
        /// <summary>
        /// Query all objects
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll();
        
        /// <summary>
        /// Get object with specific id
        /// </summary>
        /// <param name="id">Id - int or long</param>
        /// <returns></returns>
        Task<T> Get(object id);
        
        /// <summary>
        /// Add entity to the repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> Add(T entity);
        
        /// <summary>
        /// Update existing entity in the repository
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> Update(T entity);

        /// <summary>
        /// Add list of entities
        /// </summary>
        /// <param name="entities">list of entities</param>
        /// <returns></returns>
        Task<ICollection<T>> AddAll(ICollection<T> entities);
        
        /// <summary>
        /// Delete entity with specific id from the repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> Delete(object id);
    }
}