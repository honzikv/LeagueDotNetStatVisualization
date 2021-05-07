namespace dotNetMVCLeagueApp.Controllers.Forms {
    public class MatchListFilterForm {
        public string Name { get; set; }
        public string Server { get; set; }
        public int NumberOfGames { get; set; }
        public string Queue { get; set; }

        public override string ToString() {
            return
                $"{nameof(Name)}: {Name}, {nameof(Server)}: {Server}, {nameof(NumberOfGames)}: {NumberOfGames}, {nameof(Queue)}: {Queue}";
        }
    }
}