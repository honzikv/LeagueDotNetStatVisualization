using System.Collections.Generic;
using dotNetMVCLeagueApp.Const;
using MingweiSamuel.Camille.SummonerV4;

namespace dotNetMVCLeagueApp.Services.Utils {
    /// <summary>
    /// Trida pro ulozeni celkovych poctu pri GetGameListStatsViewModel
    /// </summary>
    public class GameListStats {

        public List<int> Kills { get; } = new();

        public List<int> Deaths { get; } = new();

        public List<int> Assists { get; } = new();
        public List<double> KillParticipations { get; } = new();

        public Dictionary<string, int> Roles { get; } = new() {
            // Frekvenci roli muzeme sledovat napr. pomoci dictionary (nebo polem)
            {GameConstants.Top, 0},
            {GameConstants.Mid, 0},
            {GameConstants.Adc, 0},
            {GameConstants.Sup, 0},
            {GameConstants.Jg, 0}
        };

        public List<double> GoldDiffsAt10 { get; } = new();

        public List<double> CsPerMinuteList { get; } = new();
    };
}