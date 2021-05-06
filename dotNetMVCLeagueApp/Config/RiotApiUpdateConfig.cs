using System;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Config {
    public class RiotApiUpdateConfig {
        public TimeSpan MinUpdateTimeSpan { get; }
        
        /// <summary>
        /// Maximalni doba, do ktere je mozno hledat - napr. 30 dni - hry starsi nez 30 dni se uz hledat
        /// nebudou
        /// </summary>
        public TimeSpan MaxMatchAgeInDays { get; }

        public RiotApiUpdateConfig(TimeSpan minUpdateTimeSpan, TimeSpan maxMatchAgeInDays) {
            MinUpdateTimeSpan = minUpdateTimeSpan;
            MaxMatchAgeInDays = maxMatchAgeInDays;
        }

        public bool IsSummonerUpdateable(SummonerModel summoner) =>
            DateTime.Now - summoner.LastUpdate > MinUpdateTimeSpan;


        public TimeSpan GetNextUpdateTime(SummonerModel summoner) =>
            (summoner.LastUpdate + MinUpdateTimeSpan - DateTime.Now).Duration();
    }
}