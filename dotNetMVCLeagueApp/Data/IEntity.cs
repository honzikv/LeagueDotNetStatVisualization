using System.ComponentModel.DataAnnotations;

namespace dotNetMVCLeagueApp.Data {
    
    /// <summary>
    /// Base entity class
    /// </summary>
    public interface IEntity {
        
        [Key]
        int Id { get; set; }
    }
}