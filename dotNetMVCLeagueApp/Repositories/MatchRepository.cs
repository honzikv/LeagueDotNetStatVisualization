using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Utils.Exceptions;
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
                .Where(matchSummoner => matchSummoner.SummonerModelId == summoner.Id)
                .OrderByDescending(match => match.Match.PlayTime)
                .Skip(start) // Preskocime prvnich "start" prvku
                .Take(n) // max N prvku
                .ToList()
                .ConvertAll(matchSummoner => matchSummoner.Match);

        /// <summary>
        /// Funkce pro smazani starych zapasu
        /// </summary>
        /// <param name="maxAge">Maximalni doba, do ktere zapas nesmazeme</param>
        /// <returns>Seznam smazanych zapasu</returns>
        /// <exception cref="ActionNotSuccessfulException">Chyba s databazi</exception>
        public async Task<List<MatchModel>> DeleteOldMatches(DateTime maxAge) {
            List<MatchModel> oldGames;
            try {
                oldGames = LeagueDbContext.MatchModels.Where(match => match.PlayTime < maxAge).ToList();

                var oldGameLinks = new List<MatchToSummonerModel>();

                // Ziskame vsechny linky, ktere jsou relevantni k danym hram a odstranime je
                foreach (var linksForMatch in oldGames.Select(
                    match => LeagueDbContext.MatchToSummonerModels.Where(matchSummoner =>
                        matchSummoner.MatchModelId == match.Id).ToList())) {
                    linksForMatch.ForEach(link => oldGameLinks.Add(link));
                }

                // Smazeme linky a hry
                LeagueDbContext.MatchToSummonerModels.RemoveRange(oldGameLinks);
                
                // Hry "kaskadove" smazou i ostatni zavisle objekty
                LeagueDbContext.MatchModels.RemoveRange(oldGames);
                await LeagueDbContext.SaveChangesAsync();
            }
            catch (Exception ex) {
                throw new ActionNotSuccessfulException(ex.Message);
            }

            return oldGames;
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
                .Where(matchSummoner => matchSummoner.SummonerModelId == summoner.Id &&
                                        matchSummoner.Match.QueueType.ToLower() == queueType.ToLower())
                .OrderByDescending(match => match.Match.PlayTime)
                .Skip(start) // Preskocime start prvku
                .Take(n) // Max N prvku
                .ToList() // Chceme list (nebo cokoliv co je enumerable)
                .ConvertAll(matchSummoner => matchSummoner.Match); // Mapping na MatchModel
    }
}