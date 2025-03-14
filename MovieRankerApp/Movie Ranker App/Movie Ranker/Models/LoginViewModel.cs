using System.ComponentModel.DataAnnotations;

namespace Movie_Ranker.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")] //displays Remember me instead of RememberMe
        public bool RememberMe { get; set; }
    }
}
