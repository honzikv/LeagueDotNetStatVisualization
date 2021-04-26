using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Repositories;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba pro ziskani informaci o hrach
    /// </summary>
    public class MatchHistoryService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchInfoEntityRepository matchInfoEntityRepository;
        private readonly MatchInfoSummonerInfoRepository matchSummonerRepository;

        private readonly ILogger<MatchHistoryService> logger;

        public MatchHistoryService(RiotApiRepository riotApiRepository,
            MatchInfoEntityRepository matchInfoEntityRepository,
            MatchInfoSummonerInfoRepository matchSummonerRepository,
            ILogger<MatchHistoryService> logger) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchInfoEntityRepository = matchInfoEntityRepository;
            this.matchSummonerRepository = matchSummonerRepository;
        }

        /// <summary>
        /// Ziska seznam zapasu pro daneho uzivatele
        /// </summary>
        /// <param name="summoner">summoner, pro ktereho match list hledame</param>
        /// <param name="numberOfGames">pocet her</param>
        /// <returns></returns>
        public List<MatchInfoModel> GetGameMatchList(SummonerInfoModel summoner, int numberOfGames) {
            logger.LogDebug($"Getting games for {summoner}");
            var games = matchInfoEntityRepository.GetNLastMatches(summoner, numberOfGames).ToList();
            return games; // return list
        }


        /// <summary>
        /// Pridani nebo update MatchInfo
        /// </summary>
        /// <param name="summonerInfo">Summoner pro ktereho provadime update</param>
        /// <param name="apiMatchInfo">Match info z api</param>
        /// <returns></returns>
        private async Task<MatchInfoModel> AddOrUpdateMatchInfo(SummonerInfoModel summonerInfo,
            MatchInfoModel apiMatchInfo) {
            // Zkusime pridat match info do db pokud jeste nebylo pridano a ziskame vysledek
            var matchInfo = await matchInfoEntityRepository.Get(apiMatchInfo.Id) ??
                            await matchInfoEntityRepository.Add(apiMatchInfo);

            logger.LogDebug($"Summoner: {summonerInfo}, matchInfo: {matchInfo.Id}");

            // Pridame link pokud neexistuje
            if (!await matchSummonerRepository.AnyJoinBetweenMatchSummoner(matchInfo.Id, summonerInfo.Id)) {
                await matchSummonerRepository.Add(new MatchInfoSummonerInfo {
                    MatchInfoModelId = matchInfo.Id,
                    SummonerInfoModelId = summonerInfo.Id,
                    MatchInfo = matchInfo,
                    SummonerInfo = summonerInfo
                });
            }

            return await matchInfoEntityRepository.Get(apiMatchInfo.Id);
        }

        /// <summary>
        /// Synchronizace funkce UpdateGameMatchList pro controller
        /// </summary>
        /// <param name="summonerInfo"></param>
        /// <param name="numberOfGames"></param>
        /// <returns></returns>
        public List<MatchInfoModel> UpdateGameMatchListAsync(SummonerInfoModel summonerInfo, int numberOfGames)
            => UpdateGameMatchList(summonerInfo, numberOfGames).GetAwaiter().GetResult();

        /// <summary>
        /// Ziska match list z Riot api a prida ho do Db
        /// </summary>
        /// <param name="summoner">Summoner, pro ktereho zapasy hledame</param>
        /// <param name="numberOfGames">Pocet zapasu, ktery se ma nacist</param>
        /// <returns></returns>
        private async Task<List<MatchInfoModel>> UpdateGameMatchList(SummonerInfoModel summoner, int numberOfGames) {
            // Await from api
            logger.LogDebug("Getting games from API");
            var games = await riotApiRepository.GetMatchListFromApi(summoner.EncryptedAccountId,
                Region.Get(summoner.Region),
                numberOfGames);


            logger.LogDebug("Games from API obtained, updating/adding to db");
            // Nektere zaznamy o hrach uz mohou v databazi existovat, proto misto toho pridame pouze hrace k dane hre
            // aby bylo v DB co nejmene udaju
            var tasks = new List<Task<MatchInfoModel>>(games.Count);
            foreach (var game in games) {
                // Pro kazdy zapas bud pridame do databaze nebo update v join tabulce MatchInfoSummonerInfo
                tasks.Add(AddOrUpdateMatchInfo(summoner, game));
            }

            var result = await Task.WhenAll(tasks); // Pockame dokud nebudou vsechny akce hotove
            logger.LogDebug("All tasks were awaited, match list is updated and ready to be sent");
            return result.ToList();
        }
    }
}