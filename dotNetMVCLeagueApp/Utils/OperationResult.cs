namespace dotNetMVCLeagueApp.Utils {
    /// <summary>
    /// Objekt, ktery obsahuje data o dane operaci v nejake sluzbe
    /// </summary>
    public class OperationResult<T> {
        
        /// <summary>
        /// (Chybova) zprava
        /// </summary>
        public T Message { get; set; }
        
        /// <summary>
        /// Zda-li se jedna o chybu
        /// </summary>
        public bool Error { get; set; }

        /// <summary>
        /// Bezparam. konstruktor
        /// </summary>
        public OperationResult() {
            Error = false;
        }

        public OperationResult(bool error, T message) {
            Error = error;
            Message = message;
        }
    }
}