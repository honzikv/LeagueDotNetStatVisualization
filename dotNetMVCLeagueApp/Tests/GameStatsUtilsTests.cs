using System.Collections.Generic;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Services.Utils;
using Xunit;

namespace dotNetMVCLeagueApp.Tests {
    /// <summary>
    /// Obsahuje testy tridy GameStatsUtils, ktera tvori vetsinu vypoctu pro statistiky
    /// </summary>
    public class GameStatsUtilsTests {
        [Fact]
        public void GetLargestMultiKill_Penta() {
            var playerStats = new PlayerStatsModel {
                PentaKills = 1,
                QuadraKills = 2,
                DoubleKills = 1
            };

            var expected = GameConstants.PentaKill;
            var actual = GameStatsUtils.GetLargestMultiKill(playerStats);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLargestMultiKill_None() {
            var playerStats = new PlayerStatsModel();

            var expected = (string) default;
            var actual = GameStatsUtils.GetLargestMultiKill(playerStats);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetKillParticipation_Default() {
            var blueSide = 100;

            var playerInfoList = new List<PlayerInfoModel> {
                new() {
                    TeamId = blueSide,
                    PlayerStatsModel = new() {
                        Kills = 12
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStatsModel = new() {
                        Kills = 5
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStatsModel = new() {
                        Kills = 3
                    }
                }
            };

            var matchInfo = new MatchInfoModel {
                PlayerInfoList = playerInfoList
            };

            var player = playerInfoList[2]; // hrac, pro ktereho pocitame kill participation
            var expected = 3.0 / (12 + 5 + 3);
            var actual = GameStatsUtils.GetKillParticipation(player.PlayerStatsModel, matchInfo, blueSide);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetKillParticipation_NoKills() {
            var blueSide = 100;

            var playerInfoList = new List<PlayerInfoModel> {
                new() {
                    TeamId = blueSide,
                    PlayerStatsModel = new() {
                        Kills = 0
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStatsModel = new() {
                        Kills = 0
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStatsModel = new() {
                        Kills = 0
                    }
                }
            };

            var player = playerInfoList[0]; // Hrac, pro ktereho pocitame kill participation
            var expected = 1.0;
            var matchInfo = new MatchInfoModel {
                PlayerInfoList = playerInfoList
            };
            var actual = GameStatsUtils.GetKillParticipation(player.PlayerStatsModel, matchInfo, blueSide);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetTwoMostPlayedRoles_TopMid() {
            var stats = new StatTotals {
                Roles = {
                    [GameConstants.Top] = 10, [GameConstants.Mid] = 8, [GameConstants.Jg] = 1, [GameConstants.Adc] = 3
                }
            };

            var mostPlayedRoles = GameStatsUtils.GetTwoMostPlayedRoles(stats.Roles);
            Assert.Equal(mostPlayedRoles.Item1, GameConstants.Top);
            Assert.Equal(mostPlayedRoles.Item2, GameConstants.Mid);
        }
    }
}