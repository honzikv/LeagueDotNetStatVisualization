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
        /// <returns>Vrati summoner info, nebo null, pokud neexistuje</returns>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public async Task<SummonerModel> GetSummonerInfo(string summonerName, Region region) {
            try {
                var summoner = await riotApi.SummonerV4.GetBySummonerNameAsync(region, summonerName);
                if (summoner is null) { // vratime-li null, znamena to, ze summoner neexistuje
                    return null;
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
            catch (Exception ex) {
                logger.LogCritical(ex.Message); // Log chyby
                throw new ActionNotSuccessfulException(ex.Message);
            }
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
                return leagueEntries.Where(leagueEntry => leagueEntry.QueueType == GameConstants.RankedFlex ||
                                                          leagueEntry.QueueType == GameConstants.RankedSolo)
                    .Select(leagueEntry => mapper.Map<QueueInfoModel>(leagueEntry))
                    .ToList();
            }
            catch (Exception ex) {
                logger.LogCritical(ex.Message); // Log zpravy
                throw new ActionNotSuccessfulException(ex.Message);
            }
        }

        private MatchModel MapToMatchInfo(Match match) {
            var result = mapper.Map<MatchModel>(match); // mapping Match na MatchInfoModel
            result.Id = match.GameId; // Nastavime id
            result.PlayTime = TimeUtils.ConvertFromMillisToDateTime(match.GameCreation);
            result.QueueType = GameConstants.GetQueueNameFromQueueId(match.QueueId);
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

            var playerInfo = mapper.Map<PlayerModel>(participant);
            var playerStats = mapper.Map<PlayerStatsModel>(participant.Stats);

            playerInfo.PlayerStats = playerStats;

            var player = participantIdentity.Player; // Player obsahuje cast dat, ktere chceme ulozit
            playerInfo.ProfileIcon = player.ProfileIcon;
            playerInfo.SummonerId = player.SummonerId;
            playerInfo.SummonerName = player.SummonerName;
            playerInfo.ParticipantId = participantIdentity.ParticipantId;

            // Ziskani timeline pro dulezita data
            var timeline = participant.Timeline;

            // Mapping role a linky
            playerInfo.Role = timeline.Role;
            playerInfo.Lane = timeline.Lane;

            // Ziskani cs diff @ 10 a gold diff @ 10
            if (timeline.CsDiffPerMinDeltas is not null && timeline.CsDiffPerMinDeltas.ContainsKey("0-10")) {
                playerInfo.CsDiffAt10 = timeline.CsDiffPerMinDeltas["0-10"];
            }

            if (timeline.GoldPerMinDeltas is not null && timeline.GoldPerMinDeltas.ContainsKey("0-10")) {
                playerInfo.GoldDiffAt10 = timeline.GoldPerMinDeltas["0-10"];
            }

            return playerInfo;
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
                var matches = await riotApi.MatchV4.GetMatchlistAsync(
                    region: region,
                    encryptedAccountId: encryptedAccountId,
                    beginIndex: beginIdx,
                    endIndex: endIdx + numberOfGames,
                    queue: GameConstants.RelevantQueues // Chceme pouze blind pick, flex, draft a soloq
                );

                // Pokud zadne hry nejsou, vratime prazdny seznam
                if (matches is null) {
                    logger.LogDebug("Matches are null");
                    return new();
                }

                // List se ziskanim kazdeho zapasu - muzeme prinest kazdy async, protoze to bude rychlejsi
                var matchTasks = new List<Task<Match>>(matches.TotalGames);
                matchTasks.AddRange(
                    matches.Matches
                        .Select(match => riotApi.MatchV4.GetMatchAsync(region, match.GameId))
                );

                // Pockame na vsechny
                var awaitedMatches = await Task.WhenAll(matchTasks);
                var matchList = new List<MatchModel>(matches.TotalGames);
                matchList.AddRange(awaitedMatches.Select(MapToMatchInfo));

                // Namapujeme kazdy zaznam na MatchInfoModel

                logger.LogDebug("Matchlist mapped");

                return matchList;
            }

            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                throw new ActionNotSuccessfulException(ex.Message);
            }
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