using System.Collections.Generic;
using AutoMapper;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using MingweiSamuel.Camille.LeagueV4;
using MingweiSamuel.Camille.MatchV4;
using MingweiSamuel.Camille.SpectatorV4;

namespace dotNetMVCLeagueApp.Data.Mappings {
    /// <summary>
    /// Automapper config
    /// </summary>
    public class AutoMapperConfig : Profile{

        public AutoMapperConfig() {
            // Mapping from -> to
            CreateMap<LeagueEntry, QueueInfoModel>();
            CreateMap<Match, MatchInfoModel>();
            CreateMap<ChampionBanModel, BannedChampion>();
            CreateMap<ChampionBanModel[], List<BannedChampion>>();
            CreateMap<TeamStats, TeamStatsInfoModel>();
        }
    }
}