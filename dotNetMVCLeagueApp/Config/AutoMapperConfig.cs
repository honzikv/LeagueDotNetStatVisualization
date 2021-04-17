using System;
using AutoMapper;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using dotNetMVCLeagueApp.Data.ViewModels.SummonerProfile;
using MingweiSamuel.Camille.LeagueV4;
using MingweiSamuel.Camille.MatchV4;
using Participant = MingweiSamuel.Camille.MatchV4.Participant;

namespace dotNetMVCLeagueApp.Config {
    /// <summary>
    /// Automapper config pro snazsi mapping mezi objekty
    /// </summary>
    public class AutoMapperConfig : Profile {
        public AutoMapperConfig() {
                
            // Mapping from -> to
            CreateMap<LeagueEntry, QueueInfoModel>();
            CreateMap<Match, MatchInfoModel>();
            CreateMap<TeamStats, TeamStatsInfoModel>();
            CreateMap<Participant, PlayerInfoModel>();
            CreateMap<ParticipantStats, PlayerStatsModel>();
            CreateMap<MatchInfoModel, MatchInfoHeaderViewModel>();
        }
    }

}