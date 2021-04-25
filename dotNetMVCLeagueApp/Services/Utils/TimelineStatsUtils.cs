using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.ViewModels.MatchDetail;

namespace dotNetMVCLeagueApp.Services.Utils {
    /// <summary>
    /// Obsahuje vetsinu kodu pro vypocet statistik pro timeline
    /// </summary>
    public class TimelineStatsUtils {
        public static void AddMatchTimeFrame(MatchTimelineStatsViewModel result, MatchFrameModel matchTimeFrame) {
            foreach (var participantFrame in matchTimeFrame.ParticipantFrames) {
                var participantIdx = participantFrame.ParticipantId - 1; // Participant index je indexovany od 1

                var participantData = result.PlayerStats[participantIdx];
                participantData.
            }
        }
    }
}