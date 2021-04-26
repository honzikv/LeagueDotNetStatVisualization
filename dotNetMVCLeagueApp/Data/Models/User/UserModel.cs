﻿using System.ComponentModel.DataAnnotations;
using dotNetMVCLeagueApp.Data.Models.SummonerPage;

namespace dotNetMVCLeagueApp.Data.Models.User {
    /// <summary>
    ///     Model pro registraci uzivatele
    /// </summary>
    public class UserModel {
        public int Id { get; set; }

        [Display(Name = "Username")]
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter password once more")]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required] public string Email { get; set; }

        [Required] public virtual SummonerInfoModel SummonerInfo { get; set; }
    }
}