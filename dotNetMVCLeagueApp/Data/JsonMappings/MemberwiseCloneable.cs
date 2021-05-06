namespace dotNetMVCLeagueApp.Data.JsonMappings {
    public abstract class MemberwiseCloneable<T> where T : class {
        /// <summary>
        /// Vytvori shallow kopii a vrati ji
        /// </summary>
        /// <returns>Shallow kopii daneho objektu</returns>
        public T Clone() => (T) MemberwiseClone();
    }
}