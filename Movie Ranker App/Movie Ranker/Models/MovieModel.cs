using System.ComponentModel;
using System.ComponentModel.DataAnnotations; //used for data annotations like key, required etc 

namespace Movie_Ranker.Models
{
    public class MovieModel
    {
        [Key]   //need to set id as primary key else EF will not be able to track and manage the entity
        public int Id { get; set; }
        [Display(Name = "Movie Name")]           //allows us to display Movie Name instead of MovieName

        [Required]
        public string? MovieName { get; set; }

        [Required]
        public string? Genre { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode =true)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public string? Studio {  get; set; }

        [Range(0, 100, ErrorMessage ="Value must be between 0 and 100")]
        public int Score { get; set; }
    }
}
