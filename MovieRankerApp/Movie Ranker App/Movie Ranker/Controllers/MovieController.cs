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

        public IActionResult Index(string searchString = "") //search string is optional here
        {
            if (!string.IsNullOrEmpty(searchString))
            { 
                //trim the search string to remove leading or trailing spaces
                searchString = searchString.Trim();

                //get all movies from the db
                var movies = from m in _db.Movies select m;

                //filter categories if a search term is provided
                movies = movies.Where(m => m.MovieName.ToLower().Contains(searchString.ToLower()));

                //convert the filtered movies to list
                IEnumerable<MovieModel> objFilteredMovieList = movies.ToList();

                if (!objFilteredMovieList.Any())
                {
                    TempData["error"] = "No categories found. Press search again to go back";
                }

                //pass it to the view
                return View(objFilteredMovieList);
            }

            IEnumerable<MovieModel> objMovieList = _db.Movies.ToList(); //retrieves all the movies from the database and stores them in objMovieList
            var moviesByScoringOrder = objMovieList.OrderByDescending(m => m.Score).ToList(); //using LINQ we are ording the list in descending order as per score
            return View(moviesByScoringOrder); //returns the view with the list of movies
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

                @TempData["success"] = "Movie created successfully"; //stores a message in the TempData dictionary

                return RedirectToAction("Index"); //redirects to the Index action
            }
            //ensure the model passed back contains all properties
            return View(new MovieModel
            {
                MovieName = movie.MovieName,
                Genre = movie.Genre,
                ReleaseDate = movie.ReleaseDate,
                Studio = movie.Studio,
                Score = movie.Score
            }); //returns the view with the movie model
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

                TempData["success"] = "Movie has been edited successfully";

                return RedirectToAction("Index");  //redirects to the Index action
            }
            return View(movie); //returns the view with the movie model
        }

        //get method to delete a movie
        public IActionResult Delete(int id)
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

        [HttpPost]
        [ValidateAntiForgeryToken] // used to validate anti forgery
        public IActionResult DeletePost(int id)
        {
            var movie = _db.Movies.Find(id); //finds the movie with the id passed in the parameter
            if (movie == null)
            {
                return NotFound();
            }
            _db.Movies.Remove(movie); //removes the movie from the database
            _db.SaveChanges(); //saves the changes to the database

            TempData["success"] = "Movie has been deleted successfully";
            return RedirectToAction("Index"); //redirects to the Index action
        }
    }
}
