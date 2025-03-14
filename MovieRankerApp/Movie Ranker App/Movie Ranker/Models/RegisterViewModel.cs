using System.ComponentModel.DataAnnotations;

namespace Movie_Ranker.Models
{
    public class RegisterViewModel
    {
        [Required]      //built in validation attributes
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]       //masks the password input
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]    //displays Confirm password instead of ConfirmPassword
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] //compares the password and confirm password fields
        public string ConfirmPassword { get; set; }
    }
}
