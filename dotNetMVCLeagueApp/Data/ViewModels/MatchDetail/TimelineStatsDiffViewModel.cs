using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail {
    /// <summary>
    /// Rozdil hrace, pro ktereho statistiku zobrazujeme a ostatnich
    /// - mohli bychom pouzit i pole, nicmene by se museli navic prepocitavat indexy a pro tento rozsah je
    /// Dictionary ekvivalentne rychle
    /// </summary>
    public class TimelineStatsDiffViewModel {
        
        public Dictionary<int, TimeValue<int>> MaxXpDiff { get; set; }
        
        public Dictionary<int, TimeValue<int>> MinXpDiff { get; set; }
        
        public Dictionary<int, TimeValue<int>> MaxGoldDiff { get; set; }
        
        public Dictionary<int, TimeValue<int>> MinGoldDiff { get; set; }
        
        public Dictionary<int, TimeValue<int>> MinCsDiff { get; set; }
        
        public Dictionary<int, TimeValue<int>> MaxCsDiff { get; set; }
    }
}