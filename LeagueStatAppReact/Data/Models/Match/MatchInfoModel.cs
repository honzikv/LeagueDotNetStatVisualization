using System;
using System.Collections.Generic;
using LeagueStatAppReact.Data.Models.SummonerPage;

namespace LeagueStatAppReact.Data.Models.Match {
    public class MatchInfoModel : IEntity {

        public int Id { get; set; }
        
        public virtual SummonerInfoModel SummonerInfoModel { get; set; }
        
        /// <summary>
        /// Whether the player won
        /// </summary>
        public bool Win { get; set; }

        /// <summary>
        /// Id of the team - blue or red
        /// Legal values: 100 === blue, 200 === red
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// List of all bans
        /// </summary>
        public virtual IEnumerable<TeamStatsInfoModel> Teams { get; set; }

        /// <summary>
        /// Date and time when the game was played
        /// </summary>
        public DateTime PlayTime { get; set; }

        public virtual IEnumerable<PlayerInfoModel> PlayerInfoList { get; set; }

        public override string ToString() => $"Win={Win} TeamId={TeamId} PlayTime={PlayTime}";
    }
}