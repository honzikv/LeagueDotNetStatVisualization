using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotNetMVCLeagueApp.Const;
using LeagueStatAppReact.Data.Models.Match;
using LeagueStatAppReact.Data.Models.SummonerPage;
using LeagueStatAppReact.Exceptions;
using Microsoft.Extensions.Logging;
using MingweiSamuel.Camille;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.MatchV4;

namespace LeagueStatAppReact.Repositories {
    /// <summary>
    /// This repository wraps functionality of RiotApi object for better abstraction
    /// </summary>
    public class RiotApiRepository {
        /// <summary>
        /// This error message shows to user whenever there is some problem with the API
        /// The real error is logged in debug level
        /// </summary>
        private const string ApiErrorMessage = "There was an error while communicating with the Riot API";

        private readonly RiotApi riotApi;
        private readonly IMapper mapper;

        private readonly ILogger<RiotApiRepository> logger;

        public RiotApiRepository(RiotApi riotApi, IMapper mapper, ILogger<RiotApiRepository> logger) {
            this.riotApi = riotApi;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<SummonerInfoModel> GetSummonerInfo(string summonerName, Region region) {
            try {
                var summoner = await riotApi.SummonerV4.GetBySummonerNameAsync(region, summonerName);
                if (summoner is null) { // return null, this means that the user does not exist
                    return null;
                }

                // Map to model object
                return new SummonerInfoModel {
                    SummonerLevel = summoner.SummonerLevel,
                    Name = summoner.Name,
                    ProfileIconId = summoner.ProfileIconId,
                    EncryptedSummonerId = summoner.Id,
                    EncryptedAccountId = summoner.AccountId,
                    Region = region.Key,
                    LastUpdate = DateTime.Now
                };
            }
            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                throw new ActionNotSuccessfulException(ApiErrorMessage);
            }
        }

        /// <summary>
        /// Get list of QueueInfoModels that contain information about flexq and soloq
        /// </summary>
        /// <param name="encryptedSummonerId">Encrypted summoner id</param>
        /// <param name="region">Region of the user</param>
        /// <returns>List of QueueInfoModels</returns>
        public async Task<List<QueueInfoModel>> GetQueueInfoList(string encryptedSummonerId, Region region) {
            try {
                var leagueEntries =
                    await riotApi.LeagueV4.GetLeagueEntriesForSummonerAsync(region, encryptedSummonerId);

                // Return empty QueueInfo list since there are no league entries in the api
                if (leagueEntries is null) {
                    return new();
                }

                // Iterate over all entries, map flex queue and solo queue to the rankedInfoModel object
                return leagueEntries.Where(leagueEntry => leagueEntry.QueueType == LeagueEntryConst.RankedFlex ||
                                                          leagueEntry.QueueType == LeagueEntryConst.RankedSolo)
                    .Select(leagueEntry => mapper.Map<QueueInfoModel>(leagueEntry))
                    .ToList();
            }
            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                throw new ActionNotSuccessfulException(ApiErrorMessage);
            }
        }

        private MatchInfoModel MapToMatchInfo(Match match) {
            var result = mapper.Map<MatchInfoModel>(match); // shallow map from Match to MatchInfo object
            logger.LogDebug(result.ToString());

            // Map nested objects - team stats and players
            var teams = new List<TeamStatsInfoModel>();
            // Map TeamStatsInfoModel objects
            foreach (var team in match.Teams) {
                var teamStatsInfo = mapper.Map<TeamStatsInfoModel>(team);

                var bans = new List<ChampionBanModel>(team.Bans.Length);
                foreach (var ban in team.Bans) {
                    bans.Add(mapper.Map<ChampionBanModel>(ban));
                }
                
                teamStatsInfo.Bans = bans;
                teams.Add(teamStatsInfo);
            }

            // Map participants
            var players = new List<PlayerInfoModel>();
            foreach (var participantIdentity in match.ParticipantIdentities) {
                var playerInfo = MapParticipantToPlayer(match, participantIdentity);
                players.Add(playerInfo);
            }

            result.Teams = teams;
            result.PlayerInfoList = players;
            return result;
        }

        private PlayerInfoModel MapParticipantToPlayer(Match match, ParticipantIdentity participantIdentity) {
            var participant = match.Participants
                .FirstOrDefault(x => x.ParticipantId == participantIdentity.ParticipantId);

            if (participant == null) {
                throw new RiotApiError("Error when mapping participant");
            }

            var playerInfo = mapper.Map<PlayerInfoModel>(participant);
            var playerStats = mapper.Map<PlayerStatsModel>(participant.Stats);

            playerInfo.PlayerStatsModel = playerStats;

            // Get CsPerMinute, GoldDiffAt10, CsDiffPer10 and role from timeline
            var timeline = participant.Timeline;

            // Calculate cs per minute if present
            if (timeline.CreepsPerMinDeltas.Count > 0) {
                playerInfo.CsPerMinute = timeline.CreepsPerMinDeltas.Values.Sum() /
                                         timeline.CreepsPerMinDeltas.Values.Count;
            }

            playerInfo.Role = timeline.Role;
            playerInfo.Lane = timeline.Lane;
            // timeline.CsDiffPerMinDeltas.TryGetValue("0-10", out var csDiffAt10);
            // playerInfo.CsDiffAt10 = csDiffAt10;
            // timeline.GoldPerMinDeltas.TryGetValue("0-10", out var goldDiffAt10);
            // playerInfo.GoldDiffAt10 = goldDiffAt10;
            
            return playerInfo;
        }

        public async Task<List<MatchInfoModel>> GetMatchListFromApi(string encryptedAccountId, Region region,
            int numberOfGames)
            => await GetMatchListFromApi(encryptedAccountId, region, numberOfGames, null, 0);

        public async Task<List<MatchInfoModel>>
            GetMatchListFromApi(string encryptedAccountId, Region region, int numberOfGames, int? beginIdx, int endIdx) {
            try {
                var matches = await riotApi.MatchV4.GetMatchlistAsync(
                    region: region,
                    encryptedAccountId: encryptedAccountId,
                    beginIndex: beginIdx,
                    endIndex: endIdx + numberOfGames
                );

                // If there are no games returned then simply return an empty list
                if (matches is null) {
                    return new();
                }
                
                // List of tasks with each match
                var matchTasks = new List<Task<Match>>(matches.TotalGames);
                foreach (var match in matches.Matches) {
                    matchTasks.Add(riotApi.MatchV4.GetMatchAsync(region, match.GameId));
                }

                var matchList = new List<MatchInfoModel>(matches.TotalGames);
                foreach (var matchTask in matchTasks) {
                    matchList.Add(MapToMatchInfo(await matchTask));
                }
                
                logger.LogDebug("Matchlist mapped");

                return matchList;
            }

            catch (Exception ex) {
                logger.LogCritical(ex.Message);
                throw new ActionNotSuccessfulException(ApiErrorMessage);
            }
        }
    }
}