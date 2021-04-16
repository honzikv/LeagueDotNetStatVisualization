using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Utils;
using dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile;
using dotNetMVCLeagueApp.Exceptions;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Services {
    /// <summary>
    /// Sluzba pro vypocty statistik pro zobrazeni na strance
    /// </summary>
    public class SummonerProfileStatsService {
        private IMapper mapper;

        public SummonerProfileStatsService(IMapper mapper) {
            this.mapper = mapper;
        }

        public MatchInfoHeaderViewModel GetMatchInfoHeader(SummonerInfoModel summonerInfo,
            MatchInfoModel matchInfo) {
            var playerInfo = matchInfo.PlayerInfoList
                .FirstOrDefault(player => player.SummonerId == summonerInfo.EncryptedSummonerId);

            if (playerInfo is null) {
                throw new ActionNotSuccessfulException("Error while obtaining the data from the database");
            }

            var playerStats = playerInfo.PlayerStatsModel;
            
            // Mapping do jednoho objektu
            return new MatchInfoHeaderViewModel {
                PlayTime = matchInfo.PlayTime,
                ChampionIconId = playerInfo.ChampionId,
                TeamId = playerInfo.TeamId,
                Win = matchInfo.Teams.FirstOrDefault(team => team.Id == playerInfo.TeamId)?.Win,
                QueueType = matchInfo.GameType,
                Items = new() {
                    playerStats.Item0, playerStats.Item1, playerStats.Item2, playerStats.Item3, playerStats.Item4,
                    playerStats.Item5, playerStats.Item6
                },
                LargestMultiKill = StatsCalculationUtils.GetLargestMultiKill(playerStats),
                CsPerMinute = StatsCalculationUtils.GetCsPerMinute(playerStats.TotalMinionsKilled, matchInfo.GameDuration),
                TotalCs = playerStats.TotalMinionsKilled,
                SummonerSpell1Id = playerInfo.Spell1Id,
                SummonerSpell2Id = playerInfo.Spell2Id,
                VisionScore = playerStats.VisionScore,
                PrimaryRuneId = playerStats.Perk0,
                SecondaryRuneId = playerStats.Perk3,
            };

        }
    }
}