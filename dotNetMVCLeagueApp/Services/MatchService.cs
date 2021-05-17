using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Repositories;
using dotNetMVCLeagueApp.Utils;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Services {
    public class MatchService {
        private readonly RiotApiRepository riotApiRepository;
        private readonly MatchRepository matchRepository;
        private readonly MatchInfoSummonerInfoRepository matchSummonerRepository;
        private readonly RiotApiUpdateConfig riotApiUpdateConfig;
        private readonly SummonerRepository summonerRepository;

        private readonly ILogger<MatchService> logger;

        public MatchService(RiotApiRepository riotApiRepository,
            MatchRepository matchRepository,
            MatchInfoSummonerInfoRepository matchSummonerRepository,
            SummonerRepository summonerRepository,
            RiotApiUpdateConfig riotApiUpdateConfig,
            ILogger<MatchService> logger
        ) {
            this.riotApiRepository = riotApiRepository;
            this.logger = logger;
            this.matchRepository = matchRepository;
            this.matchSummonerRepository = matchSummonerRepository;
            this.riotApiUpdateConfig = riotApiUpdateConfig;
            this.summonerRepository = summonerRepository;
        }

        private readonly Dictionary<string, int> queueNameToQueueId = ServerConstants.QueueIdToQueueNames;

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

            logger.LogDebug($"Match id {match.Id}, PlayTime: {match.PlayTime}, queue: {match.QueueType}");

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

        public List<MatchModel> GetFrontPage(SummonerModel summoner)
            => matchRepository.GetNMatchesByDateTimeDesc(summoner, ServerConstants.DefaultNumberOfGamesInProfile);

        public Task<List<MatchModel>> GetSpecificPage(SummonerModel summoner, int pageNumber, int numberOfGames, int[] queues) {
            var toSkip = pageNumber * numberOfGames; // pocet prvku, ktere preskocime
            
            // Nyni budeme brat od aktualniho datumu az mesic zpet
            // Bohuzel, Riot API nedovoluje, abychom udelali query kde je rozsah casu vetsi nez tyden, takze
            // potrebujeme zavolat api az 4x abychom ziskali vsechny hry.
            var toDate = DateTime.Now; // Datum DO ktereho hledame - v riot api jako endTime
            var maxFromDate = toDate.Subtract(riotApiUpdateConfig.MaxMatchAgeDays);
            var run = true;
            while (run) {
                var fromDate = toDate.SubtractWeek(); // datum OD ktereho hledame - v riot api jako startTime
                if (fromDate < maxFromDate) {
                    fromDate = maxFromDate;
                    run = false;
                }

                
            }
        }
    }
}