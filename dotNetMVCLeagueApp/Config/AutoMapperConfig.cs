using System;
using AutoMapper;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
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
            CreateMap<Match, MatchModel>();
            CreateMap<TeamStats, TeamStatsModel>();
            CreateMap<Participant, PlayerModel>();
            CreateMap<MatchPosition, MapPositionModel>();
            CreateMap<ParticipantStats, PlayerStatsModel>();
            CreateMap<MatchModel, MatchInfoHeaderViewModel>();
            CreateMap<MatchParticipantFrame, MatchParticipantFrameModel>();
            CreateMap<MatchEvent, MatchEventModel>();
        }
    }

}