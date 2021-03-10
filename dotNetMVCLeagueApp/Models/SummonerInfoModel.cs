using System;
using System.ComponentModel.DataAnnotations;
using MingweiSamuel.Camille.Enums;

namespace dotNetSpLeagueApp.Models {
    public class SummonerInfoModel {
        
        /// <summary>
        /// Entity id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// In-game name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Last update of this entity
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        /// Region of the player
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// In-game level
        /// </summary>
        public long Level { get; set; }

        /// <summary>
        /// Profile icon id for correct image
        /// </summary>
        public int ProfileIconId { get; set; }

        /// <summary>
        /// Solo queue rank
        /// </summary>
        public int SoloqRank { get; set; }

        /// <summary>
        /// Flex queue rank
        /// </summary>
        public int FlexqRank { get; set; }
    }
}