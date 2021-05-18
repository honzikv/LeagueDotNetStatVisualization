namespace dotNetMVCLeagueApp.Pages.Data.MatchDetail {
    /// <summary>
    ///     Objekt, ktery obsahuje dulezite informace oproti oponentovi
    ///     Tento objekt muze byt null, pokud nejsou k dispozici informace o lince.
    ///     Nektera data poskytuje api uz primo, nicmene je snazsi je vypocitat a navic nejsou vzdy dostupna
    /// </summary>
    public class OpponentStatsDto {
        public bool LaneWon { get; set; }

        public double XpDiffAt10 { get; set; }

        public double XpDiffAt15 { get; set; }

        public double XpDiffAt20 { get; set; }

        public int ControlWardsBought { get; set; }

        public double GoldDiffAt10 { get; set; }

        public double GoldDiffAt15 { get; set; }

        public double GoldDiffAt20 { get; set; }

        public int LevelDiffAt10 { get; set; }

        public int LevelDiffAt15 { get; set; }

        public int LevelDiffAt20 { get; set; }
    }
}