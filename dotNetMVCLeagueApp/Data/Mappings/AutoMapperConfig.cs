using AutoMapper;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;
using MingweiSamuel.Camille.LeagueV4;

namespace dotNetMVCLeagueApp.Data.Mappings {
    /// <summary>
    /// Automapper config
    /// </summary>
    public class AutoMapperConfig : Profile{

        public AutoMapperConfig() {
            CreateMap<LeagueEntry, QueueInfoModel>();
        }
    }
}