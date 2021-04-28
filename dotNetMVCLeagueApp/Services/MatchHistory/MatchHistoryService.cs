using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services.MatchHistory {
    /// <summary>
    /// Sluzba pro ziskani informaci o hrach
    /// </summary>
    public class MatchHistoryService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchRepository matchRepository;
        private readonly MatchInfoSummonerInfoRepository matchSummonerRepository;
        private readonly MatchTimelineRepository matchTimelineRepository;
        private readonly SummonerRepository summonerRepository;

        private readonly RiotApiUpdateConfig apiUpdateConfig;

        private readonly ILogger<MatchHistoryService> logger;

        public MatchHistoryService(
            RiotApiRepository riotApiRepository,
            MatchRepository matchRepository,
            MatchInfoSummonerInfoRepository matchSummonerRepository,
            MatchTimelineRepository matchTimelineRepository,
            SummonerRepository summonerRepository,
            RiotApiUpdateConfig apiUpdateConfig,
            ILogger<MatchHistoryService> logger
        ) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchTimelineRepository = matchTimelineRepository;
            this.matchRepository = matchRepository;
            this.matchSummonerRepository = matchSummonerRepository;
            this.summonerRepository = summonerRepository;
            this.apiUpdateConfig = apiUpdateConfig;
        }

        /// <summary>
        /// Ziska seznam zapasu pro daneho uzivatele
        /// </summary>
        /// <param name="summoner">summoner, pro ktereho match list hledame</param>
        /// <param name="numberOfGames">pocet her</param>
        /// <returns></returns>
        public List<MatchModel> GetGameMatchList(SummonerModel summoner, int numberOfGames) {
            logger.LogDebug($"Getting games for {summoner}");
            var games = matchRepository.GetNLastMatches(summoner, numberOfGames).ToList();
            return games; // return list
        }

        /// <summary>
        /// Pridani nebo update MatchInfo
        /// </summary>
        /// <param name="summoner">Summoner pro ktereho provadime update</param>
        /// <param name="apiMatch">Match info z api</param>
        /// <returns></returns>
        private async Task<MatchModel> AddOrUpdateMatch(SummonerModel summoner,
            MatchModel apiMatch) {
            // Zkusime pridat match info do db pokud jeste nebylo pridano a ziskame vysledek
            var match = await matchRepository.Get(apiMatch.Id);

            if (match is null) {
                var matchTimelineModel =
                    await riotApiRepository.GetMatchTimelineFromApi(apiMatch.Id, Region.Get(summoner.Region));
                apiMatch.MatchTimelineModel = matchTimelineModel;
                match = await matchRepository.Add(apiMatch);
            }

            logger.LogDebug($"Summoner: {summoner}, matchInfo: {match.Id}");

            // Pridame link pokud neexistuje
            if (!await matchSummonerRepository.AnyJoinBetweenMatchSummoner(match.Id, summoner.Id)) {
                await matchSummonerRepository.Add(new MatchToSummonerModel {
                    MatchInfoModelId = match.Id,
                    SummonerInfoModelId = summoner.Id,
                    Match = match,
                    Summoner = summoner
                });
            }

            return await matchRepository.Get(apiMatch.Id);
        }

        /// <summary>
        /// Synchronizace funkce UpdateGameMatchList pro controller
        /// </summary>
        /// <param name="summoner"></param>
        /// <param name="numberOfGames"></param>
        /// <returns></returns>
        public List<MatchModel> UpdateGameMatchListAsync(SummonerModel summoner, int numberOfGames)
            => UpdateMatchList(summoner, numberOfGames).GetAwaiter().GetResult();

        // /// <summary>
        // /// Ziska match list z Riot api a prida ho do Db
        // /// </summary>
        // /// <param name="summoner">Summoner, pro ktereho zapasy hledame</param>
        // /// <param name="numberOfGames">Pocet zapasu, ktery se ma nacist</param>
        // /// <returns></returns>
        // private async Task<List<MatchModel>> UpdateMatchList1(SummonerModel summoner, int numberOfGames) {
        //     if (!apiUpdateConfig.IsSummonerUpdateable(summoner)) {
        //         throw new ActionNotSuccessfulException(
        //             "Error, summoner cannot be updated, " +
        //             $"try again in: {apiUpdateConfig.GetNextUpdateTime(summoner).TotalSeconds} seconds.");
        //     }
        //
        //     // Await from api
        //     logger.LogDebug("Getting games from API");
        //     var games = await riotApiRepository.GetMatchListFromApi(summoner.EncryptedAccountId,
        //         Region.Get(summoner.Region),
        //         numberOfGames);
        //
        //     logger.LogDebug("Games from API obtained, updating/adding to db");
        //     // Nektere zaznamy o hrach uz mohou v databazi existovat, proto misto toho pridame pouze hrace k dane hre
        //     // aby bylo v DB co nejmene udaju
        //     var tasks = new List<Task<MatchModel>>(games.Count);
        //     tasks.AddRange(games.Select(match => AddOrUpdateMatch(summoner, match)));
        //
        //     var result = await Task.WhenAll(tasks); // Pockame dokud nebudou vsechny akce hotove
        //     logger.LogDebug("All tasks were awaited, match list is updated and ready to be sent");
        //
        //     await summonerRepository.Update(summoner);
        //
        //     return result.ToList();
        // }

        private async Task<List<MatchModel>> UpdateMatchList(SummonerModel summoner, int numberOfGames) {
            // Vyhodime exception pokud uzivatel aktualizuje profil, ktery byl nedavno aktualizovany
            if (!apiUpdateConfig.IsSummonerUpdateable(summoner)) {
                throw new ActionNotSuccessfulException(
                    "Error, summoner cannot be updated, " +
                    $"try again in: {apiUpdateConfig.GetNextUpdateTime(summoner).TotalSeconds} seconds.");
            }

            var matchList = await riotApiRepository.GetMatchListFromApi(summoner.EncryptedAccountId,
                Region.Get(summoner.Region), numberOfGames);

            var result = new List<MatchModel>();
            foreach (var match in matchList) {
                result.Add(await AddOrUpdateMatch(summoner, match));
            }
            
            summoner.LastUpdate = DateTime.Now;
            await summonerRepository.Update(summoner);

            return result;
        }
        
    }
}