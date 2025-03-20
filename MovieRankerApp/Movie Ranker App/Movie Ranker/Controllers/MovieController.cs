using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Ranker.Data;
using Movie_Ranker.Models;
using Movie_Ranker.Services;

namespace Movie_Ranker.Controllers
{
    public class MovieController : Controller
    {
        //private readonly ApplicationDbContext _db is a private field that holds a reference to the ApplicationDbContext
        //this is used to store the database context instance
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailService _emailService;

        //MovieController is a constructor that takes ApplicationDbContext as a parameter and assigns it to the private field _db
        //constructior receives ApplicatioDbContext via Dependency Injection (DI)
        public MovieController(ApplicationDbContext db, UserManager<IdentityUser> userManager, EmailService emailService)
        {
            _db = db;   //assigns the injeced db instance to the private field _db so it can be used through out the controller
            _userManager = userManager; //Injecting UserManager to get the current user's ID
            _emailService = emailService;   //Injecting EmailService to send emails
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

            //If user is logged in 
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                //Filter movies by the logged-in user
                var moviesByUser = objMovieList.Where(m => m.UserId == userId).ToList();
                var moviesByScoringOrder = moviesByUser.OrderByDescending(m => m.Score).ToList();
                return View(moviesByScoringOrder);
            }
            else
            {
                var moviesByScoringOrder = objMovieList.OrderByDescending(m => m.Score).ToList(); //using LINQ we are ording the list in descending order as per score
                return View(moviesByScoringOrder); //returns the view with the list of movies
            }
        }



        //get method to create a new movie
        [Authorize]
        public IActionResult Create()
        {
            return View(); //there is no need to retrieve any data from the db as it is simply displaying an empty form for the user to fill out so its blank
        }

        //post method to create a new movie
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // used to validate anti forgery
        public IActionResult Create(MovieModel movie) //Bind ensures only the specified props are bound from the form, prevents UserId from being validated during model binding
        {
            if (ModelState.IsValid) //checks if the model state is valid
            {
                // Log the authentication state
                var isAuthenticated = User.Identity.IsAuthenticated;
                Console.WriteLine($"Is Authenticated: {isAuthenticated}");

                //Get the current logged-in user's ID
                var userId = _userManager.GetUserId(User);

                if (userId == null)
                {
                    return RedirectToAction("Login", "Account"); //redirect to login if user is not logged in
                }

                //Assign the UserId to the movie being created
                movie.UserId = userId;

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
        [Authorize]
        public IActionResult Edit(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var movie = _db.Movies.Find(id); //finds the movie with the id passed in the parameter
            if (movie == null)
            {
                return NotFound();
            }

            //check if current user is the owner of the movie
            var userId = _userManager.GetUserId(User);
            if (movie.UserId != userId)
            {
                TempData["error"] = "You are not authorized to edit this movie.";
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        //post method to edit existing movie
        [HttpPost]
        [ValidateAntiForgeryToken] // used to validate anti forgery
        public IActionResult Edit(int id, [Bind("Id,MovieName,Genre,ReleaseDate,Studio,Score,UserId")] MovieModel movie)
        {

            if (id != movie.Id)
            {
                return NotFound();
            }

            //Check if the current user is the owner of the movie
            var userId = _userManager.GetUserId(User);
            Console.WriteLine($"user id: {userId}");
            Console.WriteLine($"Movie id: {movie.UserId}");
            if (movie.UserId != userId)
            {
                TempData["error"] = "You are not authorized to edit this movie";
                return RedirectToAction("Index");
            }


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
        [Authorize]
        public IActionResult Delete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var movie = _db.Movies.Find(id); //finds the movie with the id passed in the parameter
            if (movie == null)
            {
                return NotFound();
            }

            //check if current user is the owner of the movie
            var userId = _userManager.GetUserId(User);
            if (movie.UserId != userId)
            {
                TempData["error"] = "You are not authorized to delete this movie. Sign in to your movies";
                return RedirectToAction("Index");
            }
            return View(movie);

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

            //Check if the current user is the owner of the movie
            var userId = _userManager.GetUserId(User);
            if (movie.UserId != userId)
            {
                TempData["error"] = "You are not authorized to delete this movie. Sign in to your movies";
                return RedirectToAction("Index");
            }

            _db.Movies.Remove(movie); //removes the movie from the database
            _db.SaveChanges(); //saves the changes to the database

            TempData["success"] = "Movie has been deleted successfully";
            return RedirectToAction("Index"); //redirects to the Index action
        }

        [Authorize]
        public async Task<IActionResult> ShareViaEmail(string email)
        {
            var userId = _userManager.GetUserId(User);
            var movies = _db.Movies.Where(m => m.UserId == userId).ToList();

            if (movies.Count == 0)
            {
                TempData["error"] = "You have no movies to share";
                return RedirectToAction("Index");
            }

            //Generate HTML content for the email
            var tableRows = string.Join("\n", movies.Select(movie => $@"
                <tr>
                    <td>{movie.MovieName}</td>
                    <td>{movie.Genre}</td>
                    <td>{movie.ReleaseDate}</td>
                    <td>{movie.Studio}</td>
                    <td>{movie.Score}</td>
                </tr>"));

            var htmlContent = $@"
                <h1>My Movie List</h1>
                <p>Here's my list of favourite movies:</p>
                <table>
                    <thead>
                        <tr>
                            <th>Movie Name</th>
                            <th>Genre</th>  
                            <th>Release Year</th>
                            <th>Studio</th> 
                            <th>Score</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tableRows}
                    </tbody>
                </table>
            ";

            //Send the email
            await _emailService.SendEmailAsync(email, "My Movie List", htmlContent);

            TempData["success"] = "Movies shared successfully";
            return RedirectToAction("Index");
        }
    }
}
