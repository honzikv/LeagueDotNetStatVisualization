using System.Collections.Generic;
using AutoMapper;
using dotNetMVCLeagueApp.Const;
using dotNetMVCLeagueApp.Repositories;
using MingweiSamuel.Camille.MatchV4;
using Xunit;

namespace dotNetMVCLeagueApp.Tests {
    public class RiotApiRepositoryTests {
        /// <summary>
        /// Utilitni funkce, ktera vrati timeline s nastavenym CS@10 a gold @ 10
        /// </summary>
        /// <param name="cs10">Hodnota pro CS @ 10 - CS v prvnich 10 minutach</param>
        /// <param name="gold10">Hodnota pro gold @ 10 - zlato v prvnich 10 minutach</param>
        /// <returns></returns>
        public ParticipantTimeline GetParticipantTimeline(double cs10, double gold10) {
            var result = new ParticipantTimeline();
            result.CsDiffPerMinDeltas = new Dictionary<string, double> {
                {"10-20", 3.6},
                {"0-10", cs10},
            };
            result.GoldPerMinDeltas = new Dictionary<string, double> {
                {"10-20", 325},
                {"0-10", gold10}
            };
            result.Role = GameConstants.ROLE_TOP;
            result.Lane = GameConstants.LANE_TOP;
            return result;
        }

        [Fact]
        public void TestMapParticipantToPlayer_Default() {
            var match = new Match();

            var expectedCsd = -10;
            var expectedGold10 = -200;

            var participants = new Participant[] {
                new() {
                    ParticipantId = 1,
                    ChampionId = 2,
                    TeamId = 100, // blue side
                    Timeline = GetParticipantTimeline(expectedCsd, expectedGold10),
                    Stats = new ParticipantStats {
                        Kills = 10,
                        Deaths = 12,
                        Assists = 4,
                        ChampLevel = 14,
                        Item0 = 1,
                        Item1 = 2,
                        Item2 = 3
                    }
                }
            };

        }
    }
}