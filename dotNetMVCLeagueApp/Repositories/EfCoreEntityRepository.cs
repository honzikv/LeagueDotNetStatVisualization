using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    /// <summary>
    /// Tato trida reprezentuje entity framework core repozitar - kazdy repozitar, ktery komunikuje s databazi
    /// musi tuto tridu dedit
    /// </summary>
    /// <typeparam name="TEntity">Typ entity - DbSet v DbContextu</typeparam>
    /// <typeparam name="TContext">Databazovy kontext</typeparam>
    public abstract class EfCoreEntityRepository<TEntity, TContext>
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
        /// <param name="cardId"></param>
        /// <param name="id">id entity; je typu object, protoze nekdy se pouziva long misto intu (kvuli api)</param>
        /// <returns>Entitu s danym id</returns>
        public async Task<TEntity> Get(object id) => 
            await LeagueDbContext.Set<TEntity>().FindAsync(id);

        /// <summary>
        /// Prida objekt s entitou do databaze
        /// </summary>
        /// <param name="entity">Reference na entitu</param>
        /// <returns>Entitu z databaze</returns>
        public async Task<TEntity> Add(TEntity entity) {
            await LeagueDbContext.Set<TEntity>().AddAsync(entity);
            await LeagueDbContext.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Aktualizace entity v databazi
        /// </summary>
        /// <param name="entity">reference na entitu</param>
        /// <returns>aktualizovanou entitu</returns>
        public async Task<TEntity> Update(TEntity entity) {
            LeagueDbContext.Entry(entity).State = EntityState.Modified;
            await LeagueDbContext.SaveChangesAsync();
            return entity;
        }
        
        /// <summary>
        /// Aktualizuje seznam entit v databazi
        /// </summary>
        /// <param name="entities">Seznam entit</param>
        /// <returns>Seznam aktualizovanych entit</returns>
        public async Task<List<TEntity>> UpdateAll(List<TEntity> entities) {
            LeagueDbContext.Set<TEntity>().UpdateRange(entities);
            await LeagueDbContext.SaveChangesAsync();
            return entities;
        }

        public async Task<IEnumerable<TEntity>> DeleteAll(ICollection<TEntity> entities) {
            LeagueDbContext.Set<TEntity>().RemoveRange(entities);
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