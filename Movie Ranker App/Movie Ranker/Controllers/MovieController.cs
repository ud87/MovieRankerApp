using Microsoft.AspNetCore.Mvc;
using Movie_Ranker.Data;
using Movie_Ranker.Models;

namespace Movie_Ranker.Controllers
{
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _db is a private field that holds a reference to the ApplicationDbContext
        //this is used to store the database context instance
        private readonly ApplicationDbContext _db;

        //MovieController is a constructor that takes ApplicationDbContext as a parameter and assigns it to the private field _db
        //constructior receives ApplicatioDbContext via Dependency Injection (DI)
        public MovieController(ApplicationDbContext db)
        {
            _db = db;   //assigns the injeced db instance to the private field _db so it can be used through out the controller
        }

        public IActionResult Index()
        {
            IEnumerable<MovieModel> objMovieList = _db.Movies.ToList(); //retrieves all the movies from the database and stores them in objMovieList
            return View(objMovieList); //returns the view with the list of movies
        }
    }
}
