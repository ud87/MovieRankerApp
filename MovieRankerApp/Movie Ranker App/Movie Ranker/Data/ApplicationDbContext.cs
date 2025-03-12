
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Movie_Ranker.Models; //this is Object Relational Mapper (ORM) used to interact with database in EF

namespace Movie_Ranker.Data
{
    public class ApplicationDbContext : IdentityDbContext //IdentityDbContext is used to manage users and roles in the database
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
