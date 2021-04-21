using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using MingweiSamuel.Camille.MatchV4;

namespace dotNetMVCLeagueApp.Repositories {
    public class MatchTimelineRepository : EfCoreEntityRepository<MatchTimelineModel, LeagueDbContext> {
        public MatchTimelineRepository(LeagueDbContext leagueDbContext) : base(leagueDbContext) { }
    }
}