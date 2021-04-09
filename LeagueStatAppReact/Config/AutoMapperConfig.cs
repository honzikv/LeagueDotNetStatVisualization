﻿using System;
using AutoMapper;
using LeagueStatAppReact.Data.Models.Match;
using LeagueStatAppReact.Data.Models.SummonerPage;
using MingweiSamuel.Camille.LeagueV4;
using MingweiSamuel.Camille.MatchV4;
using Participant = MingweiSamuel.Camille.MatchV4.Participant;

namespace dotNetMVCLeagueApp.Config {
    /// <summary>
    /// Automapper config
    /// </summary>
    public class AutoMapperConfig : Profile {
        public AutoMapperConfig() {
            // Map long to date time
            CreateMap<long, DateTime>().ConvertUsing<TicksToDateTimeConverter>();
                
            // Mapping from -> to
            CreateMap<LeagueEntry, QueueInfoModel>();
            CreateMap<Match, MatchInfoModel>()
                .ForMember(info => info.PlayTime,
                    opt => opt.MapFrom(src => src.GameCreation));
            CreateMap<TeamBans, ChampionBanModel>();
            CreateMap<TeamStats, TeamStatsInfoModel>();
            CreateMap<Participant, PlayerInfoModel>();
            CreateMap<ParticipantStats, PlayerStatsModel>();
        }
    }


    public class TicksToDateTimeConverter : ITypeConverter<long, DateTime> {
        public DateTime Convert(long source, DateTime destination, ResolutionContext context) => new(source);
    }
}