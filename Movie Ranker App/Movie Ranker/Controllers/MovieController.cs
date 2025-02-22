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

        //get method to create a new movie
        public IActionResult Create()
        {
            return View(); //there is no need to retrieve any data from the db as it is simply displaying an empty form for the user to fill out so its blank
        }

        //post method to create a new movie
        [HttpPost]
        [ValidateAntiForgeryToken] // used to validate anti forgery
        public IActionResult Create(MovieModel movie)
        {
            if (ModelState.IsValid) //checks if the model state is valid
            {
                _db.Movies.Add(movie); //adds the movie to the database
                _db.SaveChanges(); //saves the changes to the database
                return RedirectToAction("Index"); //redirects to the Index action
            }
            return View(movie); //returns the view with the movie model
        }


        //get method to edit existing movie
        public IActionResult Edit(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            else
            {
                var movie = _db.Movies.Find(id); //finds the movie with the id passed in the parameter
                if (movie == null)
                {
                    return NotFound();
                }
                return View(movie);
            }          
        }

        //post method to edit existing movie
        [HttpPost]
        [ValidateAntiForgeryToken] // used to validate anti forgery
        public IActionResult Edit(MovieModel movie)
        {
            if (ModelState.IsValid)
            {
                _db.Movies.Update(movie); //updates the movie in the database
                _db.SaveChanges(); //saves the changes to the database
                return RedirectToAction("Index");  //redirects to the Index action
            }
            return View(movie); //returns the view with the movie model
        }
    }
}
