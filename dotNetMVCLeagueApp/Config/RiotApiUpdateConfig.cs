using System;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Config {
    
    /// <summary>
    /// Objekt, ktery obsahuje konfiguraci pro praci s Riot API
    /// </summary>
    public class RiotApiUpdateConfig {
        
        /// <summary>
        /// Minimalni doba, pro kterou se nemuze uzivatelsky profil  aktualizovat (pres tlacitko Update v Profilu)
        /// </summary>
        public TimeSpan MinUpdateTimeSpan { get; }
        
        /// <summary>
        /// Maximalni doba, do ktere je mozno hledat - napr. 30 dni - hry starsi nez 30 dni se uz hledat
        /// nebudou
        /// </summary>
        public TimeSpan MaxMatchAgeDays { get; }
        

        public RiotApiUpdateConfig(TimeSpan minUpdateTimeSpan, TimeSpan maxMatchAgeDays) {
            MinUpdateTimeSpan = minUpdateTimeSpan;
            MaxMatchAgeDays = maxMatchAgeDays;
        }
    }
}