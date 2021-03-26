using System;
using System.Collections.Generic;
using AutoMapper;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using MingweiSamuel.Camille.LeagueV4;
using MingweiSamuel.Camille.MatchV4;
using MingweiSamuel.Camille.SpectatorV4;
using Participant = MingweiSamuel.Camille.MatchV4.Participant;

namespace dotNetMVCLeagueApp.Config {
    /// <summary>
    /// Automapper config
    /// </summary>
    public class AutoMapperConfig : Profile {
        public AutoMapperConfig() {
            // Mapping from -> to
            CreateMap<LeagueEntry, QueueInfoModel>();
            CreateMap<Match, MatchInfoModel>()
                .IgnoreAllUnmapped()
                .ForMember(dest => dest.PlayTime, opt =>
                    opt.MapFrom(src => src.GameCreation));
            CreateMap<TeamBans, ChampionBanModel>()
                .IgnoreAllUnmapped();
            CreateMap<TeamStats, TeamStatsInfoModel>()
                .IgnoreAllUnmapped();
            CreateMap<Participant, PlayerInfoModel>()
                .IgnoreAllUnmapped();
            CreateMap<ParticipantStats, PlayerStatsModel>()
                .IgnoreAllUnmapped();
        }
    }

    public static class MappingExpressionExtensions {
        public static IMappingExpression<TSource, TDest> IgnoreAllUnmapped<TSource, TDest>(
            this IMappingExpression<TSource, TDest> expression) {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }
    }
}