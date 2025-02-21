using System.ComponentModel.DataAnnotations; //used for data annotations like key, required etc 

namespace Movie_Ranker.Models
{
    public class MovieModel
    {
        [Key]   //need to set id as primary key else EF will not be able to track and manage the entity
        public int Id { get; set; }
        public string MovieName { get; set; }
        public string Genre { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode =true)]
        public DateTime ReleaseDate { get; set; }
        public string Studio {  get; set; }
        public int Score { get; set; }
    }
}
