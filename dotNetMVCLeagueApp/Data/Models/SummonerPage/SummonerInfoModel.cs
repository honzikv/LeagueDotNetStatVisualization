using System;
using System.Collections.Generic;

namespace dotNetMVCLeagueApp.Data.Models.SummonerPage {
    public class SummonerInfoModel {
        public int Id { get; set; }

        /// <summary>
        ///     Jmeno ve hre
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Posledni aktualizace entity
        /// </summary>
        public DateTime LastUpdate { get; set; }

        /// <summary>
        ///     Encrypted summoner id pro query dat
        /// </summary>
        public string EncryptedSummonerId { get; set; }

        /// <summary>
        ///     Encrypted account id pro query dat o zapasech
        /// </summary>
        public string EncryptedAccountId { get; set; }

        /// <summary>
        ///     Region hrace - EUW, NA, ...
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        ///     In-game uroven
        /// </summary>
        public long SummonerLevel { get; set; }

        /// <summary>
        ///     Id ikonky
        /// </summary>
        public int ProfileIconId { get; set; }

        /// <summary>
        ///     Reference na queue info - pro jednotlive herni mody
        /// </summary>
        public virtual ICollection<QueueInfoModel> QueueInfo { get; set; }

        /// <summary>
        ///     Pro debug
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(LastUpdate)}: {LastUpdate}, " +
                   $"{nameof(EncryptedSummonerId)}: {EncryptedSummonerId}, {nameof(EncryptedAccountId)}: " +
                   $"{EncryptedAccountId}, {nameof(Region)}: {Region}, {nameof(SummonerLevel)}: {SummonerLevel}, " +
                   $"{nameof(ProfileIconId)}: {ProfileIconId}, {nameof(QueueInfo)}: {QueueInfo}";
        }

        /// <summary>
        ///     Provede update z novejsiho objektu ziskaneho z api
        /// </summary>
        /// <param name="apiModel">Objekt z api</param>
        public void UpdateFromApi(SummonerInfoModel apiModel) {
            Name = apiModel.Name;
            LastUpdate = DateTime.Now;
            EncryptedSummonerId = apiModel.EncryptedSummonerId;
            EncryptedAccountId = apiModel.EncryptedAccountId;
            Region = apiModel.Region;
            SummonerLevel = apiModel.SummonerLevel;
            ProfileIconId = apiModel.ProfileIconId;
        }
    }
}