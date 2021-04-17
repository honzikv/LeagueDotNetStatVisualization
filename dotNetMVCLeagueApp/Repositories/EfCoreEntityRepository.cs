﻿using System;
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
            var second = newItems as T[] ?? newItems.ToArray();
            db.Set<T>().RemoveRange(enumerable.Except(second, getKey));
            db.Set<T>().AddRange(second.Except(enumerable, getKey));
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
        /// Reference na db kontext
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly TContext LeagueDbContext;


        /// <summary>
        /// Konstruktor bude mit injektovany kontext
        /// </summary>
        /// <param name="leagueDbContext">reference pro dependency injection</param>
        protected EfCoreEntityRepository(TContext leagueDbContext) {
            this.LeagueDbContext = leagueDbContext;
        }

        /// <summary>
        /// Ziska vsechny zaznamy pro specificikou entitu
        /// </summary>
        /// <returns>IEnumerable se vsemi zaznamy v tabulce</returns>
        public async Task<IEnumerable<TEntity>> GetAll() => await LeagueDbContext.Set<TEntity>().ToListAsync();

        /// <summary>
        /// Ziska entitu podle id
        /// </summary>
        /// <param name="id">id entity; je typu object, protoze nekdy se pouziva long misto intu (kvuli api)</param>
        /// <returns>Entitu s danym id</returns>
        public async Task<TEntity> Get(object id) => await LeagueDbContext.Set<TEntity>().FindAsync(id);
        

        /// <summary>
        /// Prida objekt s entitou do databaze
        /// </summary>
        /// <param name="entity">Reference na entitu</param>
        /// <returns>Entitu z databaze</returns>
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
        /// Prida kolekci entit do databaze
        /// </summary>
        /// <param name="entities">Kolekce entit - seznam</param>
        /// <returns>Vrati stejnou kolekci, kdy entity maji validni id</returns>
        public async Task<ICollection<TEntity>> AddAll(ICollection<TEntity> entities) {
            await LeagueDbContext.Set<TEntity>().AddRangeAsync(entities);
            await LeagueDbContext.SaveChangesAsync();
            return entities;
        }

        /// <summary>
        /// Odstrani entitu z databaze
        /// </summary>
        /// <param name="id">id entity</param>
        /// <returns>null pokud entita neexistuje jinak entitu</returns>
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