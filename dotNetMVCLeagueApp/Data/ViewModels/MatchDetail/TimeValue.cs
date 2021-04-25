namespace dotNetMVCLeagueApp.Data.ViewModels.MatchDetail {
    
    /// <summary>
    /// Par cas a hodnota
    /// </summary>
    /// <typeparam name="T">Jakykoliv primitivni typ co dava smysl - double, float, int, long, short, byte ...</typeparam>
    public class TimeValue<T> {
        
        /// <summary>
        /// Cas v sekundach
        /// </summary>
        public double TimeSeconds { get; set; }
        
        public T Value { get; set; }
    }
}