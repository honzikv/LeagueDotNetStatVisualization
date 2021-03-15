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
        protected readonly TContext DbContext;

        /// <summary>
        /// Constructor may only be inherited
        /// </summary>
        /// <param name="dbContext">reference for dependency injection</param>
        protected EfCoreRepository(TContext dbContext) {
            this.DbContext = dbContext;
        }

        /// <summary>
        /// Get all records of a specific entity
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetAll() => await DbContext.Set<TEntity>().ToListAsync();

        /// <summary>
        /// Get specific entity by id
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <returns></returns>
        public async Task<TEntity> Get(int id) => await DbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity> Add(TEntity entity) {
            DbContext.Set<TEntity>().Add(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Update(TEntity entity) {
            DbContext.Entry(entity).State = EntityState.Modified;
            await DbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> Delete(int id) {
            var entity = await DbContext.Set<TEntity>().FindAsync(id);
            if (entity == null) {
                return null;
            }

            DbContext.Set<TEntity>().Remove(entity);
            await DbContext.SaveChangesAsync();
            return entity;
        }
    }
}