using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotNetMVCLeagueApp.Config;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;
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
        private readonly RiotApiUpdateConfig riotApiConfig;
        private readonly IMapper mapper;

        private readonly ILogger<RiotApiRepository> logger;

        public RiotApiRepository(RiotApi riotApi, IMapper mapper, RiotApiUpdateConfig riotApiConfig,
            ILogger<RiotApiRepository> logger) {
            this.riotApi = riotApi;
            this.mapper = mapper;
            this.riotApiConfig = riotApiConfig;
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
        /// Ziska seznam queue info, tzn. informaci o jednotlivych hernich modech, relevantni je pouze ranked flex a
        /// ranked solo
        /// </summary>
        /// <param name="encryptedSummonerId">Encrypted summoner id</param>
        /// <param name="region">Server pro ktery hledame</param>
        /// <returns>Seznam QueueInfoModel objektu</returns>
        public async Task<List<QueueInfoModel>> GetQueueInfoList(string encryptedSummonerId, Region region) {
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

        /// <summary>
        /// Namapuje Match objekt z Camille C# na MatchModel pro databazi
        /// </summary>
        /// <param name="match">Match z Camille C#</param>
        /// <returns>Namapovany MatchModel objekt</returns>
        private MatchModel MapMatchToMatchModel(Match match) {
            var result = mapper.Map<MatchModel>(match); // mapping Match na MatchInfoModel (pomoci automapperu)
            result.Id = match.GameId; // Nastavime id
            result.PlayTime = TimeUtils.ConvertFromMillisToDateTime(match.GameCreation);
            result.QueueType = ServerConstants.GetQueueNameFromQueueId(match.QueueId);
            result.MatchTimelineSearched = false;

            // Mapping vnorenych objektu - tymove statistiky a hraci
            var teams = match.Teams.Select(team => mapper.Map<TeamStatsModel>(team)).ToList();

            // Mapping participant objektu na PlayeryInfoModel objekty
            var players = match.ParticipantIdentities.Select(participantIdentity =>
                MapParticipantToPlayer(match, participantIdentity)).ToList();

            result.Teams = teams;
            result.PlayerList = players;
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

            // Ziskani cs diff a gold diff
            if (timeline.CsDiffPerMinDeltas?.ContainsKey("0-10") == true) {
                playerModel.CsDiffAt10 = timeline.CsDiffPerMinDeltas["0-10"];
            }

            if (timeline.CsDiffPerMinDeltas?.ContainsKey("10-20") == true) {
                playerModel.CsDiffAt20 = timeline.CsDiffPerMinDeltas["10-20"];
            }

            if (timeline.GoldPerMinDeltas?.ContainsKey("0-10") == true) {
                playerModel.GoldDiffAt10 = timeline.GoldPerMinDeltas["0-10"];
            }

            if (timeline.GoldPerMinDeltas?.ContainsKey("10-20") == true) {
                playerModel.GoldDiffAt20 = timeline.GoldPerMinDeltas["10-20"];
            }

            return playerModel;
        }

        /// <summary>
        /// Ziska match history pro uzivatele
        /// </summary>
        /// <param name="summoner">Ucet uzivatele</param>
        /// <param name="region">Server, na kterem se hra hrala</param>
        /// <param name="start">Datum, od ktereho hledame</param>
        /// <param name="end">Datum, do ktereho hledame</param>
        /// <param name="beginIndex">Index, od ktereho hledame</param>
        /// <param name="endIndex">Index, do ktereho hledame</param>
        /// <param name="queues">Fronty, ktere hledame</param>
        /// <returns></returns>
        public async Task<Matchlist> GetMatchHistory(SummonerModel summoner, Region region,
            DateTime? start, DateTime? end, int? beginIndex, int? endIndex, int[] queues = null) =>
            await riotApi.MatchV4.GetMatchlistAsync(
                region: region,
                encryptedAccountId: summoner.EncryptedAccountId,
                beginIndex: beginIndex,
                endIndex: endIndex,
                beginTime: start is null ? null : TimeUtils.ConvertDateTimeToMillis((DateTime) start),
                endTime: end is null ? null : TimeUtils.ConvertDateTimeToMillis((DateTime) end),
                queue: queues ?? ServerConstants.RelevantQueues
            );

        /// <summary>
        /// Ziskani zapasu z id a regionu
        /// </summary>
        /// <param name="gameId">Id hry</param>
        /// <param name="region">Server, na kterem se hra hrala</param>
        /// <returns></returns>
        public async Task<MatchModel> GetMatch(long gameId, Region region) =>
            MapMatchToMatchModel(await riotApi.MatchV4.GetMatchAsync(region, gameId));

        /// <summary>
        /// Ziska timeline pro dany zapas z API
        /// </summary>
        /// <param name="matchId">Id zapasu (id v MatchInfoModel)</param>
        /// <returns>Naplneny MatchTimeline model, ktery lze ulozit do db</returns>
        public async Task<MatchTimelineModel> GetMatchTimeline(long matchId, Region region) {
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