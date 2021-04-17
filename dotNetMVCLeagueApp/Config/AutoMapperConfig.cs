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
    /// Automapper config
    /// </summary>
    public class AutoMapperConfig : Profile {
        public AutoMapperConfig() {
            // Mapping long na DateTime
            CreateMap<long, DateTime>().ConvertUsing<TicksToDateTimeConverter>();
                
            // Mapping from -> to
            CreateMap<LeagueEntry, QueueInfoModel>();
            CreateMap<Match, MatchInfoModel>()
                .ForMember(info => info.PlayTime,
                    opt => opt.MapFrom(src => src.GameCreation));
            CreateMap<TeamStats, TeamStatsInfoModel>();
            CreateMap<Participant, PlayerInfoModel>();
            CreateMap<ParticipantStats, PlayerStatsModel>();
            CreateMap<MatchInfoModel, MatchInfoHeaderViewModel>();
        }
    }


    public class TicksToDateTimeConverter : ITypeConverter<long, DateTime> {
        public DateTime Convert(long source, DateTime destination, ResolutionContext context) => new(source);
    }
}