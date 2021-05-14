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
        
        /// <summary>
        /// Test funkce pro ziskani nejvetsiho multi killu
        /// </summary>
        [Fact]
        public void GetLargestMultiKill_Penta() {
            var playerStats = new PlayerStatsModel {
                PentaKills = 1,
                QuadraKills = 2,
                DoubleKills = 1
            };

            var expected = ServerConstants.PentaKill;
            var actual = GameStatsUtils.GetLargestMultiKill(playerStats);

            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test pro vypocet CS za minutu; pro "perfektni CS v 10. minute - 12.0"
        /// </summary>
        [Fact]
        public void TestGetCsPerMinute_PerfectCS() {
            var statsModel = new PlayerStatsModel {
                NeutralMinionsKilled = 10,
                NeutralMinionsKilledTeamJungle = 5,
                NeutralMinionsKilledEnemyJungle = 5,
                TotalMinionsKilled = 110
            };
            var gameDuration = 10 * 60; // 10 minut
            var expected = 12;
            var actual = GameStatsUtils.GetCsPerMinute(statsModel, gameDuration);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test pro vypocet CS za minutu pouze na jungle
        /// </summary>
        [Fact]
        public void TestGetCsPerMinute_JungleOnly() {
            var statsModel = new PlayerStatsModel {
                NeutralMinionsKilled = 64,
                NeutralMinionsKilledTeamJungle = 60,
                NeutralMinionsKilledEnemyJungle = 4
            };
            var gameDuration = 10 * 60; // 10 minut
            var expected = 6.4;
            var actual = GameStatsUtils.GetCsPerMinute(statsModel, gameDuration);
            
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test pro multi kill pro hrace, ktery zadny nedal
        /// </summary>
        [Fact]
        public void GetLargestMultiKill_None() {
            var playerStats = new PlayerStatsModel();

            var expected = (string) default;
            var actual = GameStatsUtils.GetLargestMultiKill(playerStats);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test pro funkci GetKillParticipation pro tym o trech hracich
        /// </summary>
        [Fact]
        public void TestGetKillParticipation_3Players() {
            var blueSide = 100;

            var playerInfoList = new List<PlayerModel> {
                new() {
                    TeamId = blueSide,
                    PlayerStats = new() {
                        Kills = 12,
                        Assists = 8
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStats = new() {
                        Kills = 5,
                        Assists = 2
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStats = new() {
                        Kills = 3,
                        Assists = 4
                    }
                }
            };

            var matchInfo = new MatchModel {
                PlayerInfoList = playerInfoList
            };

            var player = playerInfoList[2]; // hrac, pro ktereho pocitame kill participation - posledni ze seznamu
            var expected = 0.35;
            var actual = GameStatsUtils.GetKillParticipation(player.PlayerStats, matchInfo, blueSide);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test pro stav, kdy tym nemel zadne zabiti
        /// </summary>
        [Fact]
        public void TestGetKillParticipation_NoKills() {
            var blueSide = 100;

            var playerInfoList = new List<PlayerModel> {
                new() {
                    TeamId = blueSide,
                    PlayerStats = new() {
                        Kills = 0
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStats = new() {
                        Kills = 0
                    }
                },
                new() {
                    TeamId = blueSide,
                    PlayerStats = new() {
                        Kills = 0
                    }
                }
            };

            var player = playerInfoList[0]; // Hrac, pro ktereho pocitame kill participation
            var expected = 1.0;
            var matchInfo = new MatchModel {
                PlayerInfoList = playerInfoList
            };
            var actual = GameStatsUtils.GetKillParticipation(player.PlayerStats, matchInfo, blueSide);
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Test pro ziskani dvou nejhranejsich roli
        /// </summary>
        [Fact]
        public void TestGetTwoMostPlayedRoles_TopMid() {
            var stats = new GameListStats {
                Roles = {
                    [ServerConstants.Top] = 10, [ServerConstants.Mid] = 8, [ServerConstants.Jg] = 1, [ServerConstants.Adc] = 3
                }
            };

            var mostPlayedRoles = GameStatsUtils.GetTwoMostPlayedRoles(stats.Roles);
            Assert.Equal(mostPlayedRoles.Item1, ServerConstants.Top);
            Assert.Equal(mostPlayedRoles.Item2, ServerConstants.Mid);
        }
    }
}