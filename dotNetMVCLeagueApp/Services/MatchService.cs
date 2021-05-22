using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Services {
    public class MatchService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchRepository matchRepository;
        private readonly MatchInfoSummonerInfoRepository matchSummonerRepository;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;

        private readonly ILogger<MatchService> logger;

        public MatchService(RiotApiRepository riotApiRepository,
            MatchRepository matchRepository,
            MatchInfoSummonerInfoRepository matchSummonerRepository,
            RiotApiUpdateConfig riotApiUpdateConfig,
            ILogger<MatchService> logger
        ) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchRepository = matchRepository;
            this.matchSummonerRepository = matchSummonerRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
        }

        private const int MaxGamesInMatchlist = 100;

        private async Task<MatchModel> AddOrUpdateMatch(SummonerModel summoner, MatchReference matchReference) {
            var match = await matchRepository.Get(matchReference.GameId) ?? await riotApiRepository.GetMatch(
                matchReference.GameId, Region.Get(summoner.Region));

            if (!await matchSummonerRepository.AnyJoinBetweenMatchSummoner(match.Id, summoner.Id)) {
                await matchSummonerRepository.Add(new MatchToSummonerModel {
                    MatchModelId = match.Id,
                    SummonerModelId = summoner.Id,
                    Match = match,
                    Summoner = summoner
                });
            }

            return await matchRepository.Get(match.Id); // refresh entity
        }

        /// <summary>
        /// Tato metoda se vola pro detail zapasu, protoze zkontroluje zda-li se zapasu uz nacital
        /// timeline a pokud ne, nacte ho
        /// </summary>
        /// <returns>MatchModel s timeline</returns>
        public async Task<MatchModel> LoadMatchWithTimeline(long matchId, Region region) {
            var match = await matchRepository.Get(matchId) ??
                        throw new ActionNotSuccessfulException("Match does not exist or was deleted.");

            if (!match.MatchTimelineSearched) {
                var matchTimeline = await riotApiRepository.GetMatchTimeline(matchId, region);
                match.MatchTimeline = matchTimeline;
                match.MatchTimelineSearched = true;
                return await matchRepository.Update(match);
            }

            return match;
        }

        /// <summary>
        /// Ziska nefiltrovanou uvodni stranku pro profil
        /// </summary>
        /// <param name="summoner"></param>
        /// <returns></returns>
        public List<MatchModel> GetFrontPage(SummonerModel summoner)
            => matchRepository.GetNMatchesByDateTimeDesc(summoner, ServerConstants.DefaultPageSize);

        public async Task<List<MatchModel>> GetMatchHistory(SummonerModel summoner, int offset, int pageSize,
            int[] queues = null) {
            var matchReferences = new List<MatchReference>(pageSize);

            // Nyni budeme brat od aktualniho datumu az mesic zpet.
            // Bohuzel, Riot API nedovoluje, abychom udelali query kde je rozsah casu vetsi nez tyden, takze
            // potrebujeme zavolat api az 4x abychom ziskali vsechny hry.
            var toDate = DateTime.Now; // Datum DO ktereho hledame - v riot api jako endTime
            var maxFromDate = toDate.Subtract(riotApiUpdateConfig.MaxMatchAgeDays);
            var fromDate = toDate.SubtractWeek(); // datum OD ktereho hledame
            var toSkip = offset; // pocet prvku, ktere preskocime

            // maximalni index, ktery ma smysl hledat
            const int maxIdx = ServerConstants.GamesLimit - 1;

            var stop = false;
            while (!stop) {
                // Nyni jeste potrebujeme krome jednoho tydne projizdet hry, ktere jsou take limitovane - max 100.
                // Tzn budeme projizdet pro fromDate -> toDate a 0 - 99, 100 - 199, 200 - 299 ... dokud nedostaneme
                // pozadovany pocet her. Nicmene pro nas limit (20 stran) je max. pocet 200

                toSkip = await GetGamesFromGivenWeek(summoner, queues, maxIdx, fromDate, toDate, matchReferences,
                    toSkip, pageSize);
                if (matchReferences.Count == pageSize) {
                    break;
                }

                toDate = fromDate;
                var fromDateMinusWeek = fromDate.SubtractWeek();
                fromDate = fromDateMinusWeek < maxFromDate ? maxFromDate : fromDateMinusWeek;
                if (fromDate == maxFromDate) {
                    stop = true;
                }
            }

            var result = new List<MatchModel>();
            foreach (var matchReference in matchReferences) {
                result.Add(await AddOrUpdateMatch(summoner, matchReference));
            }

            return result;
        }

        /// <summary>
        /// Najde vsechny hry z daneho tydne (tyden je maximimum pro hledani v api)
        /// </summary>
        /// <param name="summoner">Uzivatel, pro ktereho hledame</param>
        /// <param name="queues">Queues, ktere se maji prohledat</param>
        /// <param name="maxIdx">Maximalni index cisla hry</param>
        /// <param name="fromDate">Datum zacatku hledani</param>
        /// <param name="toDate">Datum, do ktereho hledame</param>
        /// <param name="matchReferences">Seznam, do ktereho se data ulozi (pokud je to mozne)</param>
        /// <param name="toSkip">Pocet zapasu, ktere chceme preskocit</param>
        /// <param name="pageSize">Velikost stranky - pomoci ni vypocteme, kolik prvku zbyva</param>
        /// <returns></returns>
        private async Task<int> GetGamesFromGivenWeek(SummonerModel summoner, int[] queues, int maxIdx,
            DateTime fromDate, DateTime toDate, List<MatchReference> matchReferences, int toSkip, int pageSize) {
            for (var beginIdx = 0; beginIdx < maxIdx; beginIdx += MaxGamesInMatchlist - 1) {
                var endIdx = beginIdx + MaxGamesInMatchlist - 1;
                var matchHistory = await riotApiRepository.GetMatchHistory(
                    summoner, Region.Get(summoner.Region), fromDate, toDate, beginIdx, endIdx,
                    queues);

                if (matchHistory is null) { // Api wrapper vraci null, pokud riot api vrati 404 - zadne hry neexistuji
                    return toSkip;
                }

                var remainingGames = pageSize - matchReferences.Count;
                var gameCount = matchHistory.Matches.Length;
                logger.LogDebug($"Games remaining: {remainingGames}, Games this week: {gameCount}, Games to skip: {toSkip}");
                if (gameCount > toSkip && remainingGames > 0) {
                    var pageSizeIdx = toSkip + remainingGames; // index pokud k fromIdx pridame zbyvajici pocet her
                    // Index, do ktereho prvky pridavame
                    var toIdx = pageSizeIdx <= gameCount ? pageSizeIdx - 1 : gameCount - 1;
                    for (var i = toSkip; i <= toIdx; i += 1) {
                        matchReferences.Add(matchHistory.Matches[i]);
                    }

                    toSkip = 0;
                }
                else {
                    toSkip -= gameCount;
                }
            }
            logger.LogDebug($"Games to skip: {toSkip}");

            return toSkip;
        }

        public async Task<List<MatchModel>> GetUpdatedMatchHistory(SummonerModel summoner, int numberOfGames)
            => await GetMatchHistory(summoner, 0, numberOfGames);
    }
}