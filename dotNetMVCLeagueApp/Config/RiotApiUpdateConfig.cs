using System;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Config {
    public class RiotApiUpdateConfig {
        public TimeSpan MinUpdateTimeSpan { get; }

        public RiotApiUpdateConfig(TimeSpan minUpdateTimeSpan) {
            MinUpdateTimeSpan = minUpdateTimeSpan;
        }

        public bool IsSummonerUpdateable(SummonerModel summoner) =>
            DateTime.Now - summoner.LastUpdate > MinUpdateTimeSpan;


        public TimeSpan GetNextUpdateTime(SummonerModel summoner) =>
            (summoner.LastUpdate + MinUpdateTimeSpan - DateTime.Now).Duration();
    }
}