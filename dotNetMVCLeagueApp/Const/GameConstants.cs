using System;

namespace dotNetMVCLeagueApp.Const {
    public class GameConstants {
        public static readonly string Win = "WIN";

        public static readonly string Loss = "Fail";
        
        public static readonly string DoubleKill = "Double Kill";

        public static readonly string TripleKill = "Triple Kill";

        public static readonly string QuadraKill = "Quadra Kill";

        public static readonly string PentaKill = "Penta Kill";

        public static readonly TimeSpan
            GameDurationForRemake = TimeSpan.FromMinutes(4); // do 4 minut se bere jako remake

        public const string RoleAdc = "DUO_CARRY";
        public const string RoleSup = "DUO_SUPPORT";
        public static readonly string LANE_BOT = "BOTTOM"; // jak pro support tak pro adc

        public static readonly string ROLE_TOP = "SOLO";
        public static readonly string LANE_TOP = "TOP";

        public static readonly string ROLE_JG = "NONE";
        public static readonly string LANE_JG = "JUNGLE";

        public static readonly string ROLE_MID = "SOLO";
        public static readonly string LANE_MID = "SOLO";

        public static readonly string TOP = "TOP";
        public static readonly string MID = "MID";
        public static readonly string JG = "JG";
        public static readonly string ADC = "ADC";
        public static readonly string SUP = "SUP";
    }
}