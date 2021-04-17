using System;
using System.Collections.Generic;
using System.Linq;

namespace dotNetMVCLeagueApp.Const {
    /// <summary>
    /// Obsahuje konstanty z Riot API
    /// </summary>
    public class GameConstants {
        public static readonly string Win = "Win";

        public static readonly string Loss = "Fail";

        public static readonly string DoubleKill = "Double Kill";

        public static readonly string TripleKill = "Triple Kill";

        public static readonly string QuadraKill = "Quadra Kill";

        public static readonly string PentaKill = "Penta Kill";

        public static readonly TimeSpan
            GameDurationForRemake = TimeSpan.FromMinutes(4); // do 4 minut se bere jako remake

        public const string RoleAdc = "DUO_CARRY";
        public const string RoleSup = "DUO_SUPPORT";
        public const string LaneBot = "BOTTOM"; // jak pro support tak pro adc

        public const string RoleTop = "SOLO";
        public const string LaneTop = "TOP";

        public const string RoleJg = "NONE";
        public const string LaneJg = "JUNGLE";

        public const string RoleMid = "SOLO";
        public const string LaneMid = "MIDDLE";

        public const string Top = "TOP";
        public const string Mid = "MID";
        public const string Jg = "JG";
        public const string Adc = "ADC";
        public const string Sup = "SUP";

        private static readonly Dictionary<int, string> QueueNames = new() {
            {430, "Blind pick"}, {400, "Draft pick"},
            {440, "Flex pick"}, {420, "Solo ranked"}
        };

        public static readonly int[] RelevantQueues = {400, 420, 430, 440};

        /// <summary>
        /// Vrati nazev queue z jeho id, pokud je queueId nevalidni, vrati prazdny retezec
        /// </summary>
        /// <param name="queueId">QueueId ziskane z Match objektu</param>
        /// <returns></returns>
        public static string GetQueueNameFromQueueId(int queueId) => 
            !QueueNames.ContainsKey(queueId) ? string.Empty : QueueNames[queueId];
    };
    
}
