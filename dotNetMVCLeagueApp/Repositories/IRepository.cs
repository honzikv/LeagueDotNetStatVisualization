using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models;

namespace dotNetMVCLeagueApp.Repositories {
    /// <summary>
    /// Abstraction layer between controller and EF Core for more conscise code
    /// </summary>
    /// <typeparam name="T">Entity class</typeparam>
    public interface IRepository<T> where T : class, IEntity {
        
        /// <summary>
        /// Query all objects
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAll();
        
        /// <summary>
        /// Get object with specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> Get(int id);
        
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
        /// Delete entity with specific id from the repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> Delete(int id);
    }
}