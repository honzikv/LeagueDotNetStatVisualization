using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;

namespace dotNetMVCLeagueApp.Services.MatchHistory {
    /// <summary>
    /// Sluzba pro ziskani informaci o hrach
    /// </summary>
    public class MatchHistoryService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchInfoEntityRepository matchInfoEntityRepository;
        private readonly MatchInfoSummonerInfoRepository matchSummonerRepository;
        private readonly MatchTimelineRepository matchTimelineRepository;

        private readonly ILogger<MatchHistoryService> logger;

        public MatchHistoryService(RiotApiRepository riotApiRepository,
            MatchInfoEntityRepository matchInfoEntityRepository,
            MatchInfoSummonerInfoRepository matchSummonerRepository,
            MatchTimelineRepository matchTimelineRepository,
            ILogger<MatchHistoryService> logger) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchTimelineRepository = matchTimelineRepository;
            this.matchInfoEntityRepository = matchInfoEntityRepository;
            this.matchSummonerRepository = matchSummonerRepository;
        }

        /// <summary>
        /// Ziska seznam zapasu pro daneho uzivatele
        /// </summary>
        /// <param name="summoner">summoner, pro ktereho match list hledame</param>
        /// <param name="numberOfGames">pocet her</param>
        /// <returns></returns>
        public List<MatchModel> GetGameMatchList(SummonerModel summoner, int numberOfGames) {
            logger.LogDebug($"Getting games for {summoner}");
            var games = matchInfoEntityRepository.GetNLastMatches(summoner, numberOfGames).ToList();
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


            var matchInfo = await matchInfoEntityRepository.Get(apiMatch.Id);
            MatchTimelineModel matchTimelineModel;

            if (matchInfo is null) {
                matchTimelineModel =
                    await riotApiRepository.GetMatchTimelineFromApi(apiMatch.Id, Region.Get(summoner.Region));
                apiMatch.MatchTimelineModel = matchTimelineModel;
                matchInfo = await matchInfoEntityRepository.Add(apiMatch);
            }

            logger.LogDebug($"Summoner: {summoner}, matchInfo: {matchInfo.Id}");

            // Pridame link pokud neexistuje
            if (!await matchSummonerRepository.AnyJoinBetweenMatchSummoner(matchInfo.Id, summoner.Id)) {
                await matchSummonerRepository.Add(new MatchToSummonerModel {
                    MatchInfoModelId = matchInfo.Id,
                    SummonerInfoModelId = summoner.Id,
                    Match = matchInfo,
                    Summoner = summoner
                });
            }

            return await matchInfoEntityRepository.Get(apiMatch.Id);
        }

        /// <summary>
        /// Synchronizace funkce UpdateGameMatchList pro controller
        /// </summary>
        /// <param name="summoner"></param>
        /// <param name="numberOfGames"></param>
        /// <returns></returns>
        public List<MatchModel> UpdateGameMatchListAsync(SummonerModel summoner, int numberOfGames)
            => UpdateMatchList(summoner, numberOfGames).GetAwaiter().GetResult();

        /// <summary>
        /// Ziska match list z Riot api a prida ho do Db
        /// </summary>
        /// <param name="summoner">Summoner, pro ktereho zapasy hledame</param>
        /// <param name="numberOfGames">Pocet zapasu, ktery se ma nacist</param>
        /// <returns></returns>
        private async Task<List<MatchModel>> UpdateMatchList(SummonerModel summoner, int numberOfGames) {
            // Await from api
            logger.LogDebug("Getting games from API");
            var games = await riotApiRepository.GetMatchListFromApi(summoner.EncryptedAccountId,
                Region.Get(summoner.Region),
                numberOfGames);

            logger.LogDebug("Games from API obtained, updating/adding to db");
            // Nektere zaznamy o hrach uz mohou v databazi existovat, proto misto toho pridame pouze hrace k dane hre
            // aby bylo v DB co nejmene udaju
            var tasks = new List<Task<MatchModel>>(games.Count);
            foreach (var game in games) {
                // Pro kazdy zapas bud pridame do databaze nebo update v join tabulce MatchInfoSummonerInfo
                tasks.Add(AddOrUpdateMatch(summoner, game));
            }

            var result = await Task.WhenAll(tasks); // Pockame dokud nebudou vsechny akce hotove
            logger.LogDebug("All tasks were awaited, match list is updated and ready to be sent");
            return result.ToList();
        }
    }
}