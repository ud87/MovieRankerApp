using System.ComponentModel.DataAnnotations;

//this model is used for resetting new password for existing accounts
namespace Movie_Ranker.Models
{
    public class ShareViaEmailModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? EmailToSend { get; set; } //this is the email address to send the movie list to, will not be stored in database
    }
}
