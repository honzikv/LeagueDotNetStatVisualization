using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    public static class DbContextExtensions {
        // Extension code from:
        // https://sodocumentation.net/entity-framework-core/topic/9527/updating-a-many-to-many-relationship
        public static void TryUpdateManyToMany<T, TKey>(this DbContext db, IEnumerable<T> currentItems,
            IEnumerable<T> newItems, Func<T, TKey> getKey) where T : class {
            var enumerable = currentItems as T[] ?? currentItems.ToArray();
            db.Set<T>().RemoveRange(enumerable.Except(newItems, getKey));
            db.Set<T>().AddRange(newItems.Except(enumerable, getKey));
        }

        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other,
            Func<T, TKey> getKeyFunc) => items
            .GroupJoin(other, getKeyFunc, getKeyFunc,
                (item, tempItems) => new {item, tempItems})
            .SelectMany(t => t.tempItems.DefaultIfEmpty(),
                (t, temp) => new {t, temp})
            .Where(t => ReferenceEquals(null, t.temp) || t.temp.Equals(default(T)))
            .Select(t => t.t.item);
    }

    public abstract class EfCoreEntityRepository<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : class
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
        protected EfCoreEntityRepository(TContext leagueDbContext) {
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
        public async Task<TEntity> Get(object id) => await LeagueDbContext.Set<TEntity>().FindAsync(id);

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
        /// Add collection of entities to the database
        /// </summary>
        /// <param name="entities">Collection of entities - i.e., a list</param>
        /// <returns>The same colleciton of entities, each with corresponding Id</returns>
        public async Task<ICollection<TEntity>> AddAll(ICollection<TEntity> entities) {
            await LeagueDbContext.Set<TEntity>().AddRangeAsync(entities);
            await LeagueDbContext.SaveChangesAsync();
            return entities;
        }

        /// <summary>
        /// Remove entity from the database
        /// </summary>
        /// <param name="id">id of the entity</param>
        /// <returns>null if it does not exist or the reference to the deleted entity</returns>
        public async Task<TEntity> Delete(object id) {
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