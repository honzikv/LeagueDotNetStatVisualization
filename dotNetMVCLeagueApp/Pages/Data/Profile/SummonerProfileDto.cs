using System;
using System.Collections.Generic;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.Models.User;

namespace dotNetMVCLeagueApp.Data.FrontendDtos.Summoner {
    public class SummonerProfileDto {
        
        public string Name { get; init; }
        
        public DateTime LastUpdate { get; init; }
        
        public string Region { get; init; }
        
        public long SummonerLevel { get; init; }
        
        public QueueInfoDto SoloQueue { get; set; }
        
        public QueueInfoDto FlexQueue { get; set; }

        public List<ProfileCardModel> ProfileCards { get; }
        
        public string ProfileIconRelativeAssetPath { get; set; }
        
        
    }
}