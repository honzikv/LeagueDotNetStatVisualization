﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeagueStatAppReact.Data.Models.SummonerPage {
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
        /// Encrypted account id which is required to query data about matches
        /// </summary>
        public String EncryptedAccountId { get; set; }

        /// <summary>
        /// Region of the player
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// In-game level
        /// </summary>
        public long SummonerLevel { get; set; }

        /// <summary>
        /// Profile icon id for correct image
        /// </summary>
        public int ProfileIconId { get; set; }

        /// <summary>
        /// All objects with (ranked) information for given queue (solo queue, flex queue, ... )
        /// </summary>
        public virtual ICollection<QueueInfoModel> QueueInfo { get; set; }

        public override string ToString() => $"{Name} {EncryptedSummonerId} {Region} {SummonerLevel} {ProfileIconId}";
    }
}