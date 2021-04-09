using System.ComponentModel.DataAnnotations;

namespace LeagueStatAppReact.Data {
    
    /// <summary>
    /// Base entity class
    /// </summary>
    public interface IEntity {
        
        [Key]
        int Id { get; set; }
    }
}