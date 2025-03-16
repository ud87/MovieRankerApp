using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; //used for data annotations like key, required etc 

namespace Movie_Ranker.Models
{
    public class MovieModel
    {
        [Key]   //need to set id as primary key else EF will not be able to track and manage the entity
        public int Id { get; set; }

        [Display(Name = "Movie Name")]           //allows us to display Movie Name instead of MovieName
        [Required]
        [Column(TypeName = "text")]     //Use text instead of nvarchar(max) in the database
        public string? MovieName { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string? Genre { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode =true)]
        [Display(Name = "Release Date")]
        [Column(TypeName = "date")]     //Ensures Postgres stores the date in date format
        public DateTime ReleaseDate { get; set; }

        [Column(TypeName = "text")]
        public string? Studio {  get; set; }

        [Range(0, 100, ErrorMessage ="Value must be between 0 and 100")]
        public int Score { get; set; }

        [Required]
        public string UserId { get; set; } //stores the user id of the user who created the movie

        [ForeignKey("UserId")]  //foreign key to link to the user who created the movie
        public virtual IdentityUser User { get; set; } //navigation property to the user who created the movie

    }
}
