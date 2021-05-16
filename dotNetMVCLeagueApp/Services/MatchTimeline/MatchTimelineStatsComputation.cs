using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Data.FrontendDtos.MatchDetail;
using dotNetMVCLeagueApp.Data.FrontendDtos.MatchDetail.Timeline;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.PlayerDetail;
using dotNetMVCLeagueApp.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;

namespace dotNetMVCLeagueApp.Services.MatchTimeline {
    /// <summary>
    /// Pro prehlednost je vetsina funkcionality pro vypocet statistik ve vlastni tride, aby se nemusely objekty
    /// predavat nejakym zpusobem ve MatchTimelineStatsService
    /// </summary>
    public class MatchTimelineStatsComputation {
        /// <summary>
        /// Objekt, kam se ukladaji data pro detail o hraci
        /// </summary>
        private readonly PlayerDetailDto playerDetailDto;

        /// <summary>
        /// Objekt, kam se ukladaji timelines pro zapas
        /// </summary>
        private readonly MatchTimelineDto matchTimelineDto;

        /// <summary>
        /// MatchTimelineModel z db
        /// </summary>
        private readonly MatchTimelineModel matchTimeline;

        /// <summary>
        /// Jak dlouho (ms) trva jeden frame
        /// </summary>
        private readonly long frameDuration;

        /// <summary>
        /// Id vsech ucastniku
        /// </summary>
        private readonly List<int> participantIds;

        /// <summary>
        /// Zda-li byl MatchTimelineModel zpracovany
        /// </summary>
        private bool processed;

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

        private int GetClosestFrameIndex(TimeSpan duration) =>
            (int) Math.Round(duration.TotalMilliseconds / frameDuration);

        /// <summary>
        /// Vypocte data pro PlayerDetailDto objekt
        /// </summary>
        private void ComputePlayerDetail() {
            var playerParticipantId = playerDetailDto.ParticipantId;

            // Vypocteme min max rozdili s kazdym jinym ucastnikem
            foreach (var participantId in participantIds.Where(participantId => participantId != playerParticipantId)) {
                ComputePlayerDetailForParticipant(participantId, matchTimelineDto.PlayerTimelines[participantId]);
            }
            
            // Vypocteme rozdil v 10 a 15 minute

            var frameCount = matchTimelineDto.PlayerTimelines[participantIds[0]].CsOverTime.Count;
            var frameAt10 = GetClosestFrameIndex(TimeSpan.FromMinutes(10));
            if (frameAt10 >= frameCount) {
                return;
            }

            foreach (var participantId in participantIds.Where(participantId => participantId != playerParticipantId)) {
                ComputeDiffAt10(participantId, matchTimelineDto.PlayerTimelines[participantId], frameAt10);
            }

            var frameAt15 = GetClosestFrameIndex(TimeSpan.FromMinutes(15));
            if (frameAt15 >= frameCount) {
                return;
            }

            foreach (var participantId in participantIds.Where(participantId => participantId != playerParticipantId)) {
                ComputeDiffAt15(participantId, matchTimelineDto.PlayerTimelines[participantId], frameAt15);
            }
        }

        /// <summary>
        /// Vypocte rozdily hrace, ktereho sledujeme a ostatnich
        /// </summary>
        /// <param name="participantId">Id hrace, se kterym porovnavame</param>
        /// <param name="playerTimeline">Timeline hrace, pro ktereho sestavujeme statistiku</param>
        /// <param name="frameAt10">Index framu v 10 minute</param>
        private void ComputeDiffAt10(int participantId, PlayerTimelineDto playerTimeline, int frameAt10) {
            // Timeline hrace, vuci kteremu porovnavame
            var comparedPlayerTimeline = matchTimelineDto.PlayerTimelines[participantId];

            playerDetailDto.CsDiffAt10[participantId] =
                playerTimeline.CsOverTime[frameAt10] - comparedPlayerTimeline.CsOverTime[frameAt10];

            playerDetailDto.GoldDiffAt10[participantId] =
                playerTimeline.GoldOverTime[frameAt10] - comparedPlayerTimeline.GoldOverTime[frameAt10];

            playerDetailDto.LevelDiffAt10[participantId] =
                playerTimeline.LevelOverTime[frameAt10] - comparedPlayerTimeline.LevelOverTime[frameAt10];

            playerDetailDto.XpDiffAt10[participantId] =
                playerTimeline.XpOverTime[frameAt10] - comparedPlayerTimeline.XpOverTime[frameAt10];
        }

        /// <summary>
        /// Vypocte rozdily hrace, ktereho sledujeme a ostatnich
        /// </summary>
        /// <param name="participantId">Id hrace, se kterym porovnavame</param>
        /// <param name="playerTimeline">Timeline hrace, pro ktereho sestavujeme statistiku</param>
        /// <param name="frameAt15">Index framu v 15 minute</param>
        private void ComputeDiffAt15(int participantId, PlayerTimelineDto playerTimeline, int frameAt15) {
            // Timeline hrace, vuci kteremu porovnavame
            var comparedPlayerTimeline = matchTimelineDto.PlayerTimelines[participantId];

            playerDetailDto.CsDiffAt15[participantId] =
                playerTimeline.CsOverTime[frameAt15] - comparedPlayerTimeline.CsOverTime[frameAt15];

            playerDetailDto.GoldDiffAt15[participantId] =
                playerTimeline.GoldOverTime[frameAt15] - comparedPlayerTimeline.GoldOverTime[frameAt15];

            playerDetailDto.LevelDiffAt15[participantId] =
                playerTimeline.LevelOverTime[frameAt15] - comparedPlayerTimeline.LevelOverTime[frameAt15];

            playerDetailDto.XpDiffAt15[participantId] =
                playerTimeline.XpOverTime[frameAt15] - comparedPlayerTimeline.XpOverTime[frameAt15];
        }

        /// <summary>
        /// Prevede frameIdx (zacinajici od 0) na TimeSpan
        /// </summary>
        /// <param name="frameIdx"></param>
        /// <returns>Timespan korespondujici pro dany frame a frameDuration</returns>
        private TimeSpan FrameIndexToTimeSpan(int frameIdx) =>
            TimeUtils.ConvertFrameTimeToTimeSpan(frameDuration * (frameIdx + 1));

        private void ComputePlayerDetailForParticipant(int participantId, PlayerTimelineDto playerTimeline) {
            // Timeline hrace, vuci kteremu porovnavame
            var comparedPlayerTimeline = matchTimelineDto.PlayerTimelines[participantId];

            // Nejmene bolestivy zpusob bez pouziti reflection je odecist kazdy prvek v seznamu naseho hrace a 
            // hrace, vuci kteremu porovnavame - tzn dostaneme seznam L_diff = [L1[0] - L2[0], L1[1] - L2[1], ...]
            // Toto provedeme pro kazdou property a z vysledku zjistime minima a maxima, ktera ulozime

            var xpDiffList = playerTimeline.XpOverTime.Zip(comparedPlayerTimeline.XpOverTime,
                (first, second) => first - second).ToList();

            var goldDiffList = playerTimeline.GoldOverTime.Zip(comparedPlayerTimeline.GoldOverTime,
                (first, second) => first - second).ToList();

            var csDiffList = playerTimeline.CsOverTime.Zip(comparedPlayerTimeline.CsOverTime,
                (first, second) => first - second).ToList();

            var levelDiffList = playerTimeline.LevelOverTime.Zip(comparedPlayerTimeline.LevelOverTime,
                (first, second) => first - second).ToList();

            var (maxXpDiff, maxXpDiffIdx) = xpDiffList.MaxWithIndex();
            var (minXpDiff, minXpDiffIdx) = xpDiffList.MinWithIndex();

            playerDetailDto.MaxXpDiff[participantId] =
                new TimeValue<int>(maxXpDiff, FrameIndexToTimeSpan(maxXpDiffIdx));

            playerDetailDto.MinXpDiff[participantId] =
                new TimeValue<int>(minXpDiff, FrameIndexToTimeSpan(minXpDiffIdx));

            var (maxGoldDiff, maxGoldDiffIdx) = goldDiffList.MaxWithIndex();
            var (minGoldDiff, minGoldDiffIdx) = goldDiffList.MinWithIndex();

            playerDetailDto.MaxGoldDiff[participantId] =
                new TimeValue<int>(maxGoldDiff, FrameIndexToTimeSpan(maxGoldDiffIdx));

            playerDetailDto.MinGoldDiff[participantId] =
                new TimeValue<int>(minGoldDiff, FrameIndexToTimeSpan(minGoldDiffIdx));

            var (maxCsDiff, maxCsDiffIdx) = csDiffList.MaxWithIndex();
            var (minCsDiff, minCsDiffIdx) = csDiffList.MinWithIndex();

            playerDetailDto.MaxCsDiff[participantId] =
                new TimeValue<int>(maxCsDiff, FrameIndexToTimeSpan(maxCsDiffIdx));

            playerDetailDto.MinCsDiff[participantId] =
                new TimeValue<int>(minCsDiff, FrameIndexToTimeSpan(minCsDiffIdx));

            var (maxLevelDiff, maxLevelDiffIdx) = levelDiffList.MaxWithIndex();
            var (minLevelDiff, minLevelDiffIdx) = levelDiffList.MinWithIndex();

            playerDetailDto.MaxLevelDiff[participantId] =
                new TimeValue<int>(maxLevelDiff, FrameIndexToTimeSpan(maxLevelDiffIdx));

            playerDetailDto.MinLevelDiff[participantId] =
                new TimeValue<int>(minLevelDiff, FrameIndexToTimeSpan(minLevelDiffIdx));
        }
    }
}