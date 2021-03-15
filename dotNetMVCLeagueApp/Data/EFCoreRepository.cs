using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Data {
    public abstract class EfCoreRepository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class, IEntity
        where TContext : DbContext {

        /// <summary>
        /// Reference to the (database) context
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly TContext LeagueDbContext;

        /// <summary>
        /// The constructor may only be inherited by child classes
        /// </summary>
        /// <param name="leagueDbContext">reference for dependency injection</param>
        protected EfCoreRepository(TContext leagueDbContext) {
            this.LeagueDbContext = leagueDbContext;
        }

        /// <summary>
        /// Get all records of a specific entity
        /// </summary>
        /// <returns>list of all entities in the table</returns>
        public async Task<IEnumerable<TEntity>> GetAll() => await LeagueDbContext.Set<TEntity>().ToListAsync();

        /// <summary>
        /// Get specific entity by id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <returns>entity object</returns>
        public async Task<TEntity> Get(int id) => await LeagueDbContext.Set<TEntity>().FindAsync(id);

        /// <summary>
        /// Add entity to the database
        /// </summary>
        /// <param name="entity">reference to the entity</param>
        /// <returns></returns>
        public async Task<TEntity> Add(TEntity entity) {
            LeagueDbContext.Set<TEntity>().Add(entity);
            await LeagueDbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Update entity in the database
        /// </summary>
        /// <param name="entity">reference to the entity</param>
        /// <returns>reference to the updated entity</returns>
        public async Task<TEntity> Update(TEntity entity) {
            LeagueDbContext.Entry(entity).State = EntityState.Modified;
            await LeagueDbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Remove entity from the database
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <returns>null if it does not exist or the reference to the deleted entity</returns>
        public async Task<TEntity> Delete(int id) {
            var entity = await LeagueDbContext.Set<TEntity>().FindAsync(id);
            if (entity == null) {
                return null;
            }

            LeagueDbContext.Set<TEntity>().Remove(entity);
            await LeagueDbContext.SaveChangesAsync();
            return entity;
        }
    }
}