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
        public List<MatchModel> GetNMatchesByDateTimeDesc(SummonerModel summoner, int n, int start = 0) =>
            LeagueDbContext.MatchToSummonerModels
                .Where(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id)
                .OrderByDescending(match => match.Match.PlayTime)
                .Skip(start) // Preskocime prvnich "start" prvku
                .Take(n) // max N prvku
                .ToList()
                .ConvertAll(matchSummoner => matchSummoner.Match);

        public MatchModel GetMatchFromPageOrGetLast(SummonerModel summoner, int toSkip) {
            var matchToSummoner = LeagueDbContext.MatchToSummonerModels
                .Where(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id)
                .OrderByDescending(match => match.Match.PlayTime)
                .Skip(toSkip)
                .FirstOrDefault();

            return matchToSummoner is null
                ? LeagueDbContext.MatchToSummonerModels
                    .LastOrDefault(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id)?.Match
                : matchToSummoner.Match;
        }

        /// <summary>
        /// Ziska poslednich N zapasu pro dane queue
        /// </summary>
        /// <param name="summoner">Summoner, pro ktereho zapasy hledame</param>
        /// <param name="queueType">Typ queue</param>
        /// <param name="n">Pocet zapasu</param>
        /// <param name="start">Zacatek - kolik zapasu se preskoci</param>
        /// <returns></returns>
        public List<MatchModel> GetNMatchesByQueueTypeAndDateTimeDesc(SummonerModel summoner, string queueType, int n,
            int start = 0)
            => LeagueDbContext.MatchToSummonerModels
                .Where(matchSummoner => matchSummoner.SummonerInfoModelId == summoner.Id &&
                                        matchSummoner.Match.QueueType.ToLower() == queueType.ToLower())
                .OrderByDescending(match => match.Match.PlayTime)
                .Skip(start) // Preskocime start prvku
                .Take(n) // Max N prvku
                .ToList() // Chceme list (nebo cokoliv co je enumerable)
                .ConvertAll(matchSummoner => matchSummoner.Match); // Mapping na MatchModel
    }
}