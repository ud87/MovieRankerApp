using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Movie_Ranker.Data;
using Movie_Ranker.Models;
using Movie_Ranker.Services;
using System.Linq;
using System.Text;

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
                    TempData["error"] = "No movies found. Press search again to go back";
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



        [HttpGet]
        [Authorize]
        public IActionResult ShareViaEmail()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShareViaEmail(ShareViaEmailModel model)
        {
            var userId = _userManager.GetUserId(User);
            var movies = _db.Movies.Where(m => m.UserId == userId).ToList(); //get list by userId

            //Client side validation check
            if (!ModelState.IsValid)
            {
                return View(model);  //returns the view with validation messages
            }

            if (movies.Count == 0)
            {
                TempData["error"] = "You have no movies to share";
                return RedirectToAction("Index");
            }

            var sortedMovies = movies.OrderByDescending(movie => movie.Score).ToList(); //order by score

            //Generate HTML content for the email preview
            var tableRows = string.Join("\n", sortedMovies.Select(movie => $@"
                <tr style='border-bottom: 1px solid black; style= padding:1rem;'>
                    <td style='padding:1rem;'>{movie.MovieName}</td>
                    <td style='padding:1rem;'>{movie.Genre}</td>
                    <td style='text-align:center; style=padding:1rem;'>{movie.ReleaseDate.Year}</td>
                    <td style='text-align:center; style=padding:1rem;'>{movie.Score}</td>
                </tr>"));

            var htmlContent = $@"
                <h2 style='text-align: center'>My Movie List</h2>
                <p style='text-align: center'>Here's my list of favourite movies:</p>
                <table style='border-collapse: collapse; margin: 25px auto; font-size: 0.9em; font-family: sans-serif; min-width: 400px; box-shadow: 0 0 20px rgba(0, 0 , 0, 0.15);'>
                    <thead style='padding:1rem;'>
                        <tr style='background-color: #009879; color:#ffffff; text-align: left; style=padding:1rem;'>
                            <th style='padding:1rem;'>Movie Name</th>
                            <th style='padding:1rem;'>Genre</th>  
                            <th style='padding:1rem;'>Release Year</th>
                            <th style='padding:1rem;'>Score</th>
                        </tr>
                    </thead>
                    <tbody style:'padding: 4rem;'>
                        {tableRows}
                    </tbody>
                </table>
            ";

            //Send the email
            await _emailService.SendEmailAsync(model.EmailToSend, "My Movies List", htmlContent);

            TempData["success"] = "Movies shared successfully";
            return RedirectToAction("Index");

        }

        [HttpGet]
        [Authorize]
        public IActionResult GetEmailPreview()
        {
            var userId = _userManager.GetUserId(User);
            var movies = _db.Movies.Where(m => m.UserId == userId).ToList();

            if (movies.Count == 0)
            {
                return Content("<p>You have no movies to share</p>");
            }

            var sortedMovies = movies.OrderByDescending(movie => movie.Score).ToList();

            //Generate HTML content for the email preview
            var tableRows = string.Join("\n", sortedMovies.Select(movie => $@"
                <tr style='border-bottom: 1px solid black; style= padding:1rem;'>
                    <td style='padding:1rem;'>{movie.MovieName}</td>
                    <td style='padding:1rem;'>{movie.Genre}</td>
                    <td style='text-align:center; style=padding:1rem;'>{movie.ReleaseDate.Year}</td>
                    <td style='text-align:center; style=padding:1rem;'>{movie.Score}</td>
                </tr>"));

            var htmlContent = $@"
                <h2 style='text-align: center'>My Movie List</h2>
                <p style='text-align: center'>Here's my list of favourite movies:</p>
                <table style='border-collapse: collapse; margin: 25px auto; font-size: 0.9em; font-family: sans-serif; min-width: 400px; box-shadow: 0 0 20px rgba(0, 0 , 0, 0.15);'>
                    <thead style='padding:1rem;'>
                        <tr style='background-color: #009879; color:#ffffff; text-align: left; style=padding:1rem;'>
                            <th style='padding:1rem;'>Movie Name</th>
                            <th style='padding:1rem;'>Genre</th>  
                            <th style='padding:1rem;'>Release Year</th>
                            <th style='padding:1rem;'>Score</th>
                        </tr>
                    </thead>
                    <tbody style:'padding: 4rem;'>
                        {tableRows}
                    </tbody>
                </table>
            ";

            return Content(htmlContent, "text/html");
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ShareViaEmailModel model)
        {
            //checks if email address is valid
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid email address";
                return View(model);
            }

            //check if email exists in the database
            var user = await _userManager.FindByEmailAsync(model.EmailToSend);
            //Console.WriteLine(user != null ? "User found" : "User not found");

            if (user == null)
            {
                TempData["success"] = "If an account exists with this email, a reset link has been sent";
                //clear input field
                return RedirectToAction("ForgotPassword");
            }

            //generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            //Environment-aware URL generation
            string callBackUrl;

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                //Hardcoded for development
                callBackUrl = $"http://localhost:5209/Movie/ResetPassword?email={Uri.EscapeDataString(model.EmailToSend)}&token={encodedToken}";
            }
            else
            {
                //Dynamic for production
                callBackUrl = Url.Action(
                    "ResetPassword", 
                    "Movie",
                    new
                    {
                        email = model.EmailToSend,
                        token = encodedToken
                    },
                    protocol: Request.Scheme);
            }

            //For testing
            Console.WriteLine($"Reset link: {callBackUrl}");

            

            //send email with link
            await _emailService.SendEmailAsync(model.EmailToSend, "Reset Password",
                $"Please reset your password by clicking here: <a href='{callBackUrl}'>link</a>");

            TempData["success"] = "If an account exists with this email, a reset link has been sent";

            return RedirectToAction("ForgotPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        } 

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Get user by email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            //Decode the token
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));

            //Reset the password
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            //Console.WriteLine($"Result is {result}");
            if (result.Succeeded)
            {
                //Send confirmation email
                await _emailService.SendEmailAsync(model.Email, "Password Changed", "<p>Your password has been changed successfully.</p>");
                return RedirectToAction("Login", "Account");  //go to Account controller and login view
            }
             
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);

        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
