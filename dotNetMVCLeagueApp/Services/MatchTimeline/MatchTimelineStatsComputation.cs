using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Data.FrontendModels.MatchDetail;
using dotNetMVCLeagueApp.Data.FrontendModels.MatchDetail.PlayerDetail;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Data.ViewModels.MatchDetail;
using dotNetMVCLeagueApp.Data.ViewModels.MatchDetail.Timeline;
using dotNetMVCLeagueApp.Exceptions;
using dotNetMVCLeagueApp.Utils;

namespace dotNetMVCLeagueApp.Services.MatchTimeline {
    /// <summary>
    /// Pro prehlednost je vetsina funkcionality pro vypocet statistik ve vlastni tride, aby se nemusely objekty
    /// predavat nejakym zpusobem ve MatchTimelineStatsService
    /// </summary>
    public class MatchTimelineStatsComputation {
        private PlayerDetailDto playerDetailDto;

        private MatchTimelineDto matchTimelineDto;

        private readonly MatchTimelineModel matchTimeline;

        private long frameDuration;

        private bool processed;

        private List<int> participantIds;

        public MatchTimelineStatsComputation(List<int> participantIds, int participantId,
            int laneOpponentParticipantId, MatchTimelineModel matchTimeline) {
            this.matchTimeline = matchTimeline;
            this.participantIds = participantIds;
            playerDetailDto = new PlayerDetailDto(participantId, laneOpponentParticipantId, participantIds);
            matchTimelineDto = new MatchTimelineDto(
                participantIds,
                TimeUtils.ConvertFrameTimeToTimeSpan(matchTimeline.FrameInterval)
            );
            frameDuration = matchTimeline.FrameInterval;
        }

        /// <summary>
        /// Metoda zpracuje celou timeline a prevede ji na objekt pro frontend
        /// Tato metoda se musi zavolat pro ziskani vysledku
        /// </summary>
        public void Process() {
            // Ulozime data ze vsech framu
            foreach (var matchFrame in matchTimeline.MatchFrames) {
                ProcessFrame(matchFrame);
            }

            // Nyni mame data pro vsechny framy a muzeme zjistit extremy pro daneho hrace - to predtim v for cyklu
            // neslo, protoze bychom museli z for cyklu nejakym zpusobem skakat na data, ktera jsme jeste neprevedli
            ComputePlayerDetail();

            processed = true;
        }


        public (PlayerDetailDto, MatchTimelineDto) GetResult => !processed
            ? throw new ActionNotSuccessfulException("Error, timeline was not processed")
            : (playerDetailDto, matchTimelineDto);

        private void ProcessFrame(MatchFrameModel matchFrame) {
            foreach (var frame in matchFrame.ParticipantFrames) {
                var participantTimeline = matchTimelineDto.PlayerTimelines[frame.ParticipantId];

                // Pridani dat z aktualniho framu
                participantTimeline.CsOverTime.Add(frame.MinionsKilled + frame.JungleMinionsKilled);
                participantTimeline.GoldOverTime.Add(frame.TotalGold);
                participantTimeline.LevelOverTime.Add(frame.Level);
                participantTimeline.XpOverTime.Add(frame.Xp);
            }
        }

        private void ComputePlayerDetail() {
            var playerParticipantId = playerDetailDto.ParticipantId;

            foreach (var participantId in participantIds) {
                if (participantId == playerParticipantId) {
                    continue;
                }

                ComputePlayerDetailForParticipant(participantId, matchTimelineDto.PlayerTimelines[participantId]);
            }
        }

        private void ComputePlayerDetailForParticipant(int participantId, PlayerTimelineDto playerTimeline) {
            // Timeline hrace, vuci kteremu porovnavame
            var comparedPlayerTimeline = matchTimelineDto.PlayerTimelines[participantId];

            var xpDiffList = playerTimeline.XpOverTime.Zip(comparedPlayerTimeline.XpOverTime, 
                (first, second) => first - second);
            
            var goldDiffList = playerTimeline.GoldOverTime.Zip(comparedPlayerTimeline.GoldOverTime,
                (first, second) => first - second);

            var csDiffList = playerTimeline.CsOverTime.Zip(comparedPlayerTimeline.CsOverTime,
                (first, second) => first - second);

            var levelDiffList = playerTimeline.LevelOverTime.Zip(comparedPlayerTimeline.LevelOverTime,
                (first, second) => first - second);
            
            playerDetailDto.MaxXpDiff[participantId] = 
        }
    }
}