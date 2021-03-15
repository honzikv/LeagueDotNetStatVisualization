using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotNetMVCLeagueApp.Data;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Models {
    public class SummonerInfoModel : IEntity {

        /// <summary>
        /// Primary key
        /// </summary>
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
        /// Encrypted summoner id which is required to query data about user
        /// </summary>
        [MaxLength(63)]
        public String EncryptedSummonerId { get; set; }

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

        [ForeignKey("Id")]
        public RankedInfoModel RankedInfo { get; set; }
    }
}