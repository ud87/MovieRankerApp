using Microsoft.EntityFrameworkCore;
using Movie_Ranker.Models; //this is Object Relational Mapper (ORM) used to interact with database in EF

namespace Movie_Ranker.Data
{
    public class ApplicationDbContext : DbContext //DbContext is a class in EF Core that represents a session with the database and allows us to query and save instances of our entities
    {
        //ApplicationDbContext is a constructor that takes DbContextOptions as a parameter, this ensures that dbContext is configured correctly
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) //base is used to call the constructor of the parent class
        {

        }


        //create a table in the database for the MovieModel
        //movies is the name of the table in the database
        public DbSet<MovieModel> Movies { get; set; } 
    }
}
