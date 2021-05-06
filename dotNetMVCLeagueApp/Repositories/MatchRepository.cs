using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using Microsoft.EntityFrameworkCore;

namespace dotNetMVCLeagueApp.Repositories {
    public class MatchRepository : EfCoreEntityRepository<MatchModel, LeagueDbContext> {
        public MatchRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }

        /// <summary>
        /// Ziska poslednich N zapasu podle nejnovejsiho datumu pro daneho summonera
        /// </summary>
        /// <param name="summoner">Summoner, ktery se v zapasech vyskytoval</param>
        /// <param name="n">Pocet zapasu</param>
        /// <param name="start">Zacatek</param>
        /// <returns>Seznam s N nebo mene entitami</returns>
        public IEnumerable<MatchModel> GetNMatches(SummonerModel summoner, int n, int start = 0) =>
            LeagueDbContext.MatchToSummonerModels
                .Where(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id)
                .OrderByDescending(match => match.Match.PlayTime)
                .Skip(start)// Preskocime prvnich "start" prvku
                .Take(n) // max N prvku
                .ToList()
                .ConvertAll(matchSummoner => matchSummoner.Match);

        /// <summary>
        /// Ziska poslednich N zapasu pro dane queue
        /// </summary>
        /// <param name="summoner">Summoner, pro ktereho zapasy hledame</param>
        /// <param name="queueType">Typ queue</param>
        /// <param name="n">Pocet zapasu</param>
        /// <param name="start">Zacatek - kolik zapasu se preskoci</param>
        /// <returns></returns>
        public IEnumerable<MatchModel> GetNMatchesByQueueType(SummonerModel summoner, string queueType, int n,
            int start = 0)
            => LeagueDbContext.MatchToSummonerModels
                .Where(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id &&
                                        // Pro jistotu prevedeme obe queue na lowercase, pro realnou aplikaci
                                        // by asi davalo vetsi smysl udelat nejaky list of values v DB,
                                        // nicmene zde nam staci toto
                                        matchSummoner.Match.QueueType.ToLower() == queueType.ToLower())
                .OrderByDescending(match => match.Match.PlayTime)
                .Skip(start) // Preskocime start prvku
                .Take(n) // Max N prvku
                .ToList() // Chceme list (nebo cokoliv co je enumerable)
                .ConvertAll(matchSummoner => matchSummoner.Match); // Mapping na MatchModel
    }
}