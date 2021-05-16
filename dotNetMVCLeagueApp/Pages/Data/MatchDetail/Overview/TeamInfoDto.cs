namespace dotNetMVCLeagueApp.Data.FrontendDtos.MatchDetail.Overview {
    /// <summary>
    ///     Informace o tymu
    /// </summary>
    public class TeamInfoDto {
        /// <summary>
        ///     Jmeno tymu. Legalni hodnoty "Red" nebo "Blue"
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        ///     Zda-li tym vyhral - pro nastaveni pozadi ve view
        /// </summary>
        public bool Win { get; set; }

        /// <summary>
        ///     Celkovy pocet zlata
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        ///     Celkovy pocet zabiti
        /// </summary>
        public int TotalKills { get; set; }

        /// <summary>
        ///     Celkovy pocet smrti
        /// </summary>
        public int TotalDeaths { get; set; }

        /// <summary>
        ///     Celkovy pocet baronu
        /// </summary>
        public int Barons { get; set; }

        /// <summary>
        ///     Celkovy pocet draku
        /// </summary>
        public int Dragons { get; set; }

        /// <summary>
        ///     Pocet znicenych vezi
        /// </summary>
        public int TurretKills { get; set; }

        /// <summary>
        ///     Pocet znicenych inhibitoru
        /// </summary>
        public int InhibitorKills { get; set; }
    }
}