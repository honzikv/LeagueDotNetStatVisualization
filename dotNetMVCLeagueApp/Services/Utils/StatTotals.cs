using System.Collections.Generic;
using dotNetMVCLeagueApp.Const;

namespace dotNetMVCLeagueApp.Services.Utils {
    /// <summary>
    /// Trida pro ulozeni celkovych poctu pri GetGameListStatsViewModel
    /// </summary>
    public class StatTotals {
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
        public int TotalAssists { get; set; }
        public List<double> KillParticipations { get; } = new();

        public Dictionary<string, int> Roles { get; set; } = new() {
            // Frekvenci roli muzeme sledovat napr. pomoci dictionary (nebo polem)
            {GameConstants.TOP, 0},
            {GameConstants.MID, 0},
            {GameConstants.ADC, 0},
            {GameConstants.SUP, 0},
            {GameConstants.JG, 0}
        };

        public List<double> GoldDiffsAt10 { get; } = new();

        public List<double> CsPerMinuteList { get; } = new();
    };
}