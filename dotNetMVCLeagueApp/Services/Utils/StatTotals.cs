using System.Collections.Generic;
using dotNetMVCLeagueApp.Const;

namespace dotNetMVCLeagueApp.Services.Utils {
    /// <summary>
    /// Trida pro ulozeni celkovych poctu pri GetGameListStatsViewModel
    /// </summary>
    public class StatTotals {
        public int TotalKills { get; set; }
        
        public int MinKills { get; set; }
        
        public int MaxKills { get; set; }
        public int TotalDeaths { get; set; }
        
        public int MinDeaths { get; set; }
        
        public int MaxDeaths { get; set; }
        public int TotalAssists { get; set; }
        
        public int MinAssists { get; set; }
        
        public int MaxAssists { get; set; }
        public List<double> KillParticipations { get; } = new();

        public Dictionary<string, int> Roles { get; set; } = new() {
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