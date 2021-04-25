using System.Data.Common;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.ViewModels.MatchDetail;

namespace dotNetMVCLeagueApp.Services.Utils {
    /// <summary>
    /// Pro prehlednost je vetsina funkcionality pro vypocet statistik ve vlastni tride
    /// </summary>
    public class TimeStatsComputation {
        private readonly MatchTimelineModel matchTimelineModel;

        public MatchTimelineStatsViewModel Stats { get; }

        public TimeStatsComputation(MatchTimelineModel matchTimelineModel, int participants) {
            this.matchTimelineModel = matchTimelineModel;
            Stats = new MatchTimelineStatsViewModel(participants, matchTimelineModel.FrameInterval);
        }

        /// <summary>
        /// Vypocte statistiky
        /// </summary>
        public void CalculateStats() {
            foreach (var matchFrame in matchTimelineModel.MatchFrames) {
                UpdateParticipantFrames(matchFrame);
                UpdateEvents(matchFrame);
            }
        }

        private void UpdateParticipantFrames(MatchFrameModel matchFrame) {
            foreach (var frame in matchFrame.ParticipantFrames) {
                // Objekt se statistikami pro daneho ucastnika
                var participantStats = Stats.GetParticipant(frame.ParticipantId);
                participantStats.GoldOverTime.Add(frame.TotalGold);
                participantStats.CsOverTime.Add(frame.MinionsKilled + frame.JungleMinionsKilled);
                participantStats.LevelOverTime.Add(frame.Level);
                participantStats.XpOverTime.Add(frame.Xp);
            }
        }

        private void UpdateEvents(MatchFrameModel matchFrame) {
            
        }
    }
}