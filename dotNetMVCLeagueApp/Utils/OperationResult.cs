namespace dotNetMVCLeagueApp.Utils {
    /// <summary>
    /// Objekt, ktery obsahuje data o dane operaci v nejake sluzbe
    /// </summary>
    public class OperationResult<T> {
        
        public T Message { get; set; }
        
        public bool Error { get; set; }

        public OperationResult() {
            Error = false;
        }

        public OperationResult(bool error, T message) {
            Error = error;
            Message = message;
        }
    }
}