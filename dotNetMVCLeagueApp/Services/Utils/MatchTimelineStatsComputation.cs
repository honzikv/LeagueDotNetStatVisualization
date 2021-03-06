using System;
using System.Collections.Generic;
using System.Linq;
using dotNetMVCLeagueApp.Data.Models.Match;
using dotNetMVCLeagueApp.Data.Models.Match.Timeline;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail;
using dotNetMVCLeagueApp.Pages.Data.MatchDetail.Timeline;
using dotNetMVCLeagueApp.Utils;
using dotNetMVCLeagueApp.Utils.Exceptions;

namespace dotNetMVCLeagueApp.Services.Utils {
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
        /// Ucastnici
        /// </summary>
        private readonly List<PlayerModel> players;

        /// <summary>
        /// Zda-li byl MatchTimelineModel zpracovany
        /// </summary>
        private bool processed;

        /// <summary>
        /// Konstruktor pro objekt, pro ziskani vysledku se musi zavolat metoda Process()
        /// </summary>
        /// <param name="player">hrac, pro ktereho sestavujeme statistiku</param>
        /// <param name="laneOpponent">oponent na lince, ktereho jsme ziskali z Riot API</param>
        /// <param name="players">Hraci ve hre</param>
        /// <param name="matchTimeline">Objekt s casovou osou</param>
        public MatchTimelineStatsComputation(PlayerModel player, PlayerModel laneOpponent, List<PlayerModel> players,
            MatchTimelineModel matchTimeline) {
            this.matchTimeline = matchTimeline;
            this.players = players;
            playerDetailDto = new PlayerDetailDto(player, laneOpponent, players);
            matchTimelineDto = new MatchTimelineDto(
                players,
                TimeUtils.ConvertFrameTimeToTimeSpan(matchTimeline.FrameInterval)
            );
            frameDuration = matchTimeline.FrameInterval;
            matchTimelineDto.PlayerParticipantId = player.ParticipantId;
            matchTimelineDto.OpponentParticipantId = laneOpponent.ParticipantId;
        }

        /// <summary>
        /// Metoda zpracuje celou timeline a prevede ji na objekt pro frontend
        /// Tato metoda se musi zavolat pro ziskani vysledku
        /// </summary>
        public void Process() {
            // Nejprve seradime framy podle casoveho razitka, protoze se tak nemuselo stat
            var framesOrderedByTimestamp = matchTimeline.MatchFrames.OrderBy(frame => frame.Timestamp);
            foreach (var matchFrame in framesOrderedByTimestamp) {
                matchTimelineDto.Intervals.Add(StringUtils.FrameIntervalToSeconds(matchTimelineDto.Intervals.Count,
                    matchTimelineDto.FrameIntervalSeconds));
                ProcessFrame(matchFrame);
            }

            // Nyni mame data pro vsechny framy a muzeme zjistit extremy pro daneho hrace - to predtim v for cyklu
            // neslo, protoze bychom museli z for cyklu nejakym zpusobem skakat na data, ktera jsme jeste neprevedli
            ComputePlayerDetail();

            processed = true;
        }

        /// <summary>
        /// Getter pro vysledek
        /// </summary>
        /// <exception cref="ActionNotSuccessfulException"></exception>
        public (PlayerDetailDto, MatchTimelineDto) GetResult => !processed
            ? throw new ActionNotSuccessfulException("Error, timeline was not processed")
            : (playerDetailDto, matchTimelineDto);

        /// <summary>
        /// Zpracovani framu v casove ose a ulozeni do prislusnych struktur
        /// </summary>
        /// <param name="matchFrame"></param>
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
            var player = playerDetailDto.Player;
            var playerParticipantId = player.ParticipantId;
            
            // Timeline hrace, pro ktereho statistiku sestavujeme
            var playerTimeline = matchTimelineDto.PlayerTimelines[playerParticipantId];

            // Vypocteme min max rozdily s kazdym jinym ucastnikem
            foreach (var participant in
                players.Where(playerModel => playerModel.ParticipantId != playerParticipantId)) {
                ComputePlayerDetailForParticipant(participant.ParticipantId,
                    playerTimeline);
            }

            // Vypocteme rozdil v 10 a 15 minute
            var frameCount = matchTimelineDto.PlayerTimelines[players[0].ParticipantId].CsOverTime.Count;
            var frameAt10 = GetClosestFrameIndex(TimeSpan.FromMinutes(10));
            if (frameAt10 >= frameCount) {
                return;
            }

            foreach (var participant in
                players.Where(playerModel => playerModel.ParticipantId != playerParticipantId)) {
                
                // Vypocteme data a ulozime je do PlayerDetailDto objektu
                ComputeDiff(participant.ParticipantId, playerTimeline, frameAt10,
                    playerDetailDto.CsDiffAt10, playerDetailDto.GoldDiffAt10, playerDetailDto.LevelDiffAt10,
                    playerDetailDto.XpDiffAt10);
            }

            var frameAt15 = GetClosestFrameIndex(TimeSpan.FromMinutes(15));
            if (frameAt15 >= frameCount) {
                return;
            }

            foreach (var participant in
                players.Where(playerModel => playerModel.ParticipantId != playerParticipantId)) {
                
                // Vypocteme data a ulozime je do PlayerDetailDto objektu
                ComputeDiff(participant.ParticipantId, playerTimeline, frameAt15,
                    playerDetailDto.CsDiffAt15, playerDetailDto.GoldDiffAt15, playerDetailDto.LevelDiffAt15,
                    playerDetailDto.XpDiffAt15);
            }
        }

        /// <summary>
        /// Vypocte rozdily mezi hracem pro ktereho statistiky ziskavame a hracem s participantId
        /// </summary>
        /// <param name="participantId">participantId hrace, se kterym porovnavame</param>
        /// <param name="playerTimeline">timeline hrace, pro ktereho sestavujeme statistiku</param>
        /// <param name="frameAtN">cislo framu</param>
        /// <param name="csDiff">struktura pro ukladani rozdilu CS</param>
        /// <param name="goldDiff">struktura pro ukladani rozdilu zlata</param>
        /// <param name="levelDiff">struktura pro ukladani rozdilu urovne</param>
        /// <param name="xpDiff">struktura pro ukladani rozdilu XP</param>
        private void ComputeDiff(int participantId, PlayerTimelineDto playerTimeline, int frameAtN,
            Dictionary<int, int> csDiff, Dictionary<int, int> goldDiff, Dictionary<int, int> levelDiff,
            Dictionary<int, int> xpDiff) {
            // Timeline hrace, vuci kteremu porovnavame
            var comparedPlayerTimeline = matchTimelineDto.PlayerTimelines[participantId];

            csDiff[participantId] =
                playerTimeline.CsOverTime[frameAtN] - comparedPlayerTimeline.CsOverTime[frameAtN];

            goldDiff[participantId] =
                playerTimeline.GoldOverTime[frameAtN] - comparedPlayerTimeline.GoldOverTime[frameAtN];

            levelDiff[participantId] =
                playerTimeline.LevelOverTime[frameAtN] - comparedPlayerTimeline.LevelOverTime[frameAtN];

            xpDiff[participantId] =
                playerTimeline.XpOverTime[frameAtN] - comparedPlayerTimeline.XpOverTime[frameAtN];
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