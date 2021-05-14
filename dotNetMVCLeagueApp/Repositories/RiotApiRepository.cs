using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Utils;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Repositories {
    /// <summary>
    /// Tento repozitar slouzi jako wrapper pro komunikaci s API
    /// </summary>
    public class RiotApiRepository {
        private readonly RiotApi riotApi;
        private readonly IMapper mapper;

        private readonly ILogger<RiotApiRepository> logger;

        public RiotApiRepository(RiotApi riotApi, IMapper mapper, ILogger<RiotApiRepository> logger) {
            this.riotApi = riotApi;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Ziska summoner info, pokud dany summoner existuje
        /// </summary>
        /// <param name="summonerName">Jmeno summonera</param>
        /// <param name="region">Server, pro ktery jmeno hledame</param>
        /// <returns>Vrati summoner info</returns>
        /// <exception cref="RiotApiException">exception, pokud summoner neexistuje</exception>
        public async Task<SummonerModel> GetSummonerInfo(string summonerName, Region region) {
            var summoner = await riotApi.SummonerV4.GetBySummonerNameAsync(region, summonerName);
            if (summoner is null) { // vratime-li null, znamena to, ze summoner neexistuje
                throw new RiotApiException("Error, summoner does not exist");
            }

            // Jinak vratime namapovany objekt
            return new SummonerModel {
                SummonerLevel = summoner.SummonerLevel,
                Name = summoner.Name,
                ProfileIconId = summoner.ProfileIconId,
                EncryptedSummonerId = summoner.Id,
                EncryptedAccountId = summoner.AccountId,
                Region = region.Key,
                LastUpdate = DateTime.MinValue
            };
        }

        /// <summary>
        /// Ziska summoner info, pokud dany summoner existuje. Tato metoda je pouzitelna pouze tehdy,
        /// pokud uz mame daneho summonera z databaze, protoze potrebujeme jeho encrypted summoner id
        /// tim navic zajistime, ze kdyz se uzivatel prejmenuje, tak se zmeni i data v databazi
        /// </summary>
        /// <param name="encryptedSummonerId">zasifrovane id pro nas api klic</param>
        /// <param name="region">region, pro ktery hledame</param>
        /// <returns>aktualizovane summoner info</returns>
        /// <exception cref="RiotApiException">exception, pokud summoner neexistuje</exception>
        public async Task<SummonerModel> GetSummonerInfoFromEncryptedSummonerId(string encryptedSummonerId,
            Region region) {
            var summoner = await riotApi.SummonerV4.GetBySummonerIdAsync(region, encryptedSummonerId);
            if (summoner is null) {
                throw new RiotApiException("Error, summoner does not exist");
            }

            // Jinak vratime namapovany objekt
            return new SummonerModel {
                SummonerLevel = summoner.SummonerLevel,
                Name = summoner.Name,
                ProfileIconId = summoner.ProfileIconId,
                EncryptedSummonerId = summoner.Id,
                EncryptedAccountId = summoner.AccountId,
                Region = region.Key,
                LastUpdate = DateTime.MinValue
            };
        }

        /// <summary>
        /// 
        /// Ziska seznam queue info, tzn. informaci o jednotlivych hernich modech, relevantni je pouze ranked flex a
        /// ranked solo
        /// </summary>
        /// <param name="encryptedSummonerId">Encrypted summoner id</param>
        /// <param name="region">Server pro ktery hledame</param>
        /// <returns>Seznam QueueInfoModel objektu</returns>
        public async Task<List<QueueInfoModel>> GetQueueInfoList(string encryptedSummonerId, Region region) {
            try {
                var leagueEntries =
                    await riotApi.LeagueV4.GetLeagueEntriesForSummonerAsync(region, encryptedSummonerId);

                // Pokud je null, vratime prazdny seznam
                if (leagueEntries is null) {
                    return new();
                }

                // Jinak iterujeme pres vsechny, vybereme Flex a Solo a vratime
                return leagueEntries.Where(leagueEntry => leagueEntry.QueueType == ServerConstants.RankedFlex ||
                                                          leagueEntry.QueueType == ServerConstants.RankedSolo)
                    .Select(leagueEntry => mapper.Map<QueueInfoModel>(leagueEntry))
                    .ToList();
            }
            catch (Exception ex) {
                logger.LogCritical(ex.Message); // Log zpravy
                throw new ActionNotSuccessfulException(ex.Message);
            }
        }

        /// <summary>
        /// Namapuje Match objekt z Camille C# na domenovy MatchModel pro databazi
        /// </summary>
        /// <param name="match">Match z Camille C#</param>
        /// <returns>Namapovany MatchModel objekt</returns>
        private MatchModel MapMatchToMatchModel(Match match) {
            var result = mapper.Map<MatchModel>(match); // mapping Match na MatchInfoModel (pomoci automapperu)
            result.Id = match.GameId; // Nastavime id
            result.PlayTime = TimeUtils.ConvertFromMillisToDateTime(match.GameCreation);
            result.QueueType = ServerConstants.GetQueueNameFromQueueId(match.QueueId);
            logger.LogDebug(result.ToString());

            // Mapping vnorenych objektu - tymove statistiky a hraci
            var teams = match.Teams.Select(team => mapper.Map<TeamStatsModel>(team)).ToList();

            // Mapping participant objektu na PlayeryInfoModel objekty
            var players = match.ParticipantIdentities.Select(participantIdentity =>
                MapParticipantToPlayer(match, participantIdentity)).ToList();

            result.Teams = teams;
            result.PlayerInfoList = players;
            return result;
        }

        /// <summary>
        /// Namapuje objekt z Riot api na PlayerInfoModel, ktery lze pote ulozit do databaze
        /// </summary>
        /// <param name="match">Objekt s informacemi o zapasu (z Camille frameworku)</param>
        /// <param name="participantIdentity">Informace o ucastnikovi</param>
        /// <returns>Namapovany objekt</returns>
        /// <exception cref="RiotApiException">Pokud participant v zapasu neexistuje</exception>
        private PlayerModel MapParticipantToPlayer(Match match, ParticipantIdentity participantIdentity) {
            var participant = match.Participants
                .FirstOrDefault(x => x.ParticipantId == participantIdentity.ParticipantId);

            if (participant == null) {
                throw new RiotApiException("Error when mapping participant");
            }

            var playerModel = mapper.Map<PlayerModel>(participant);
            var playerStats = mapper.Map<PlayerStatsModel>(participant.Stats);

            playerModel.PlayerStats = playerStats;

            var player = participantIdentity.Player; // Player obsahuje cast dat, ktere chceme ulozit
            playerModel.ProfileIcon = player.ProfileIcon;
            playerModel.SummonerId = player.SummonerId;
            playerModel.SummonerName = player.SummonerName;
            playerModel.ParticipantId = participantIdentity.ParticipantId;

            // Ziskani timeline pro dulezita data
            var timeline = participant.Timeline;

            // Mapping role a linky
            playerModel.Role = timeline.Role;
            playerModel.Lane = timeline.Lane;

            // Ziskani cs diff @ 10 a gold diff @ 10
            if (timeline.CsDiffPerMinDeltas is not null && timeline.CsDiffPerMinDeltas.ContainsKey("0-10")) {
                playerModel.CsDiffAt10 = timeline.CsDiffPerMinDeltas["0-10"];
            }

            if (timeline.GoldPerMinDeltas is not null && timeline.GoldPerMinDeltas.ContainsKey("0-10")) {
                playerModel.GoldDiffAt10 = timeline.GoldPerMinDeltas["0-10"];
            }

            return playerModel;
        }

        /// <summary>
        /// Override pro posledni hry
        /// </summary>
        /// <param name="encryptedAccountId">encrypted id ziskane z SummonerInfoModel</param>
        /// <param name="region">server pro ktery hledame</param>
        /// <param name="numberOfGames">pocet her</param>
        /// <returns></returns>
        public async Task<List<MatchModel>> GetMatchListFromApi(string encryptedAccountId, Region region,
            int numberOfGames)
            => await GetMatchListFromApi(encryptedAccountId, region, numberOfGames, null, 0);

        public async Task<List<MatchModel>> GetMatchListFromApiByDateTimeDescending(string encryptedAccountId,
            Region region, int numberOfGames, DateTime start) {
            try {
                var matchlist = await riotApi.MatchV4.GetMatchlistAsync(
                    region: region,
                    encryptedAccountId: encryptedAccountId,
                    endIndex: numberOfGames,
                    endTime: TimeUtils.ConvertDateTimeToMillis(start)
                );

                return await MapApiMatchlist(matchlist, region);
            }
            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                throw new RiotApiException("There was an error while communicating with Riot Api");
            }
        }

        public async Task<List<MatchModel>> GetMatchHistoryFromDateTimeDesc(SummonerModel summoner, Region region,
            int numberOfGames, DateTime start, int? queueType = null) {
            logger.LogDebug($"Getting match history for {summoner.Name} @ {region.Key}");

            var matchHistory = await riotApi.MatchV4.GetMatchlistAsync(
                region,
                summoner.EncryptedAccountId,
                endIndex: numberOfGames,
                endTime: TimeUtils.ConvertDateTimeToMillis(start),
                queue: queueType is not null ? new[] {(int) queueType} : ServerConstants.RelevantQueues
            );
            return await MapApiMatchlist(matchHistory, region);
        }

        /// <summary>
        /// Ziska numberOfGames zapasu pro dany encryptedAccountId z daneho serveru
        /// </summary>
        /// <param name="encryptedAccountId">Ziskano z SummonerModel</param>
        /// <param name="region">Server</param>
        /// <param name="numberOfGames"></param>
        /// <param name="beginIdx">Pocatecni index - 0 znamena od nejnovejsi, 5 - od 5 nejnovejsi...</param>
        /// <param name="endIdx">Konecny index</param>
        /// <returns>Seznam s namapovanymy objekty do db</returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        private async Task<List<MatchModel>>
            GetMatchListFromApi(string encryptedAccountId, Region region, int numberOfGames, int? beginIdx,
                int endIdx) {
            try {
                logger.LogDebug($"Geting matches for {region.Key} encrypted acc id: {encryptedAccountId}");
                var matchlist = await riotApi.MatchV4.GetMatchlistAsync(
                    region: region,
                    encryptedAccountId: encryptedAccountId,
                    beginIndex: beginIdx,
                    endIndex: endIdx + numberOfGames,
                    queue: ServerConstants.RelevantQueues // Chceme pouze blind pick, flex, draft a soloq
                );
                // Pokud zadne hry nejsou, vratime prazdny seznam
                return await MapApiMatchlist(matchlist, region);
            }

            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                throw new RiotApiException("There was an error while communicating with Riot Api");
            }
        }

        private async Task<List<MatchModel>> MapApiMatchlist(Matchlist matchlist, Region region) {
            if (matchlist is null) {
                logger.LogDebug("Matches are null");
                return new();
            }

            // List se ziskanim kazdeho zapasu - muzeme prinest kazdy async, protoze to bude rychlejsi
            var matchTasks = new List<Task<Match>>(matchlist.TotalGames);
            matchTasks.AddRange(
                matchlist.Matches
                    .Select(match => riotApi.MatchV4.GetMatchAsync(region, match.GameId))
            );

            // Pockame na vsechny
            var awaitedMatches = await Task.WhenAll(matchTasks);
            var matchList = new List<MatchModel>(matchlist.TotalGames);
            matchList.AddRange(awaitedMatches.Select(MapMatchToMatchModel));

            // Namapujeme kazdy zaznam na MatchInfoModel

            logger.LogDebug("Matchlist mapped");

            return matchList;
        }

        /// <summary>
        /// Ziska timeline pro dany zapas z API
        /// </summary>
        /// <param name="matchId">Id zapasu (id v MatchInfoModel)</param>
        /// <returns>Naplneny MatchTimeline model, ktery lze ulozit do db</returns>
        public async Task<MatchTimelineModel> GetMatchTimelineFromApi(long matchId, Region region) {
            logger.LogDebug($"Getting match timeline from api for matchId: {matchId}");
            var matchTimeline = await riotApi.MatchV4.GetMatchTimelineAsync(region, matchId);

            // Pokud api nic nevratilo, vratime null
            return matchTimeline is null ? null : MapApiTimelineToMatchTimelineModel(matchTimeline);
        }

        /// <summary>
        /// Namapuje MatchTimeLine objekt na MatchTimelineModel, ktery lze ulozit do db
        /// </summary>
        /// <param name="matchTimeline">Puvodni objekt z api</param>
        /// <returns>Naplneny MatchTimeline model, ktery lze ulozit do db</returns>
        private MatchTimelineModel MapApiTimelineToMatchTimelineModel(MatchTimeline matchTimeline) =>
            new() {
                FrameInterval = matchTimeline.FrameInterval,
                MatchFrames = matchTimeline.Frames.Select(frame =>
                    new MatchFrameModel {
                        Timestamp = frame.Timestamp,
                        ParticipantFrames = frame.ParticipantFrames.Values.Select(participantFrame =>
                            mapper.Map<MatchParticipantFrameModel>(participantFrame)).ToList(),
                        MatchEvents = frame.Events.Select(matchEvent =>
                            mapper.Map<MatchEventModel>(matchEvent)).ToList()
                    }).ToList()
            };
    }
}