using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Movie_Ranker.Controllers;
using Movie_Ranker.Data;
using Movie_Ranker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

public class MovieControllerTests
{
    private readonly MovieController _controller;
    private readonly ApplicationDbContext _context;

    public MovieControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "MovieRankerDB") // ✅ Use In-Memory DB
            .Options;

        _context = new ApplicationDbContext(options); // ✅ Use real context
        _controller = new MovieController(_context);  // ✅ Inject real in-memory DB
    }

    //test if Create method in MovieController correctly returns a ViewResult with invalid movie model when ModelState is invalid
    [Fact]
    public void Create_ReturnsViewResult_WhenModelStateIsInvalid() //method under test: Create, Expected outcome: ViewResult, Test condition: When ModelState is invalid
    {
        // Arrange
        _context.Database.EnsureDeleted(); // ✅ Clears previous test data
        _context.Database.EnsureCreated(); //recreates the database

        var movie = new MovieModel //invalid model because MovieName is empty
        {
            MovieName = "",
            Genre = "Action",
            ReleaseDate = DateTime.Now.AddDays(5), //invalid future date
            Studio = "Universal",
            Score = 5
        };
        _controller.ModelState.AddModelError("MovieName", "Required");   //manually adds error to the ModelState for MovieName simulating form validation failure


        //Act
        var result = _controller.Create(movie); //calls the Create method of MovieController with invalid movie object

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result); //ensures its a viewResult
        Assert.NotNull(viewResult);   //checks viewResult is not null ensuring that controller returned a response

       
        var returnedMovie = Assert.IsType<MovieModel>(viewResult.Model);
        Assert.NotNull(returnedMovie); //ensures model is not null

        Assert.Equal(movie.MovieName, returnedMovie.MovieName); //ensures invalid model is returned
        Assert.Equal(movie.Genre, returnedMovie.Genre);
        Assert.Equal(movie.Score, returnedMovie.Score);
        Assert.Equal(movie.Studio, returnedMovie.Studio);
        Assert.Equal(movie.ReleaseDate, returnedMovie.ReleaseDate);
    }


    //test if Create method in MovieController correctly redirects to Index when ModelState is valid
    [Fact]
    public void Create_RedirectsToIndex_WhenModelStateIsValid()
    {
        //Arrange
        var movie = new MovieModel
        {
            MovieName = "Inception",
            Genre = "Action",
            ReleaseDate = DateTime.Now.AddDays(-5), //valid past date
            Studio = "Universal",
            Score = 5
        };

        //Mock of TempData to prevent null reference exception
        _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );


        //Act
        var result = _controller.Create(movie); //calls the Create method of MovieController with valid movie object
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result); //ensures its a redirectToActionResult
        Assert.Equal("Index", redirectToActionResult.ActionName); //checks if the action redirects to Index
    
        //check if movie was actually added to the database
        var addedMovie = _context.Movies.FirstOrDefault(movie => movie.MovieName == "Inception");
        Assert.NotNull(addedMovie); //ensures movie was added to the database
        Console.WriteLine($"Studio: {addedMovie.Studio}");
        Assert.Equal("Universal", addedMovie.Studio); //ensures correct movie was added, extra validation 
    }


    [Fact]
    //test if update method in MovieController correctly returns a ViewResult with invalid movie model when ModelState is invalid
    public void Edit_ReturnsViewResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var movie = new MovieModel //invalid model because MovieName is empty
        {
            Id = 1,
            MovieName = "",
            Genre = "Action",
            ReleaseDate = DateTime.Now.AddDays(5), //invalid future date
            Studio = "Universal",
            Score = 5
        };
        _controller.ModelState.AddModelError("MovieName", "Required");   //manually adds error to the ModelState for MovieName simulating form validation failure

        //Mock TempData to prevent null reference exception
        _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );

        //Act
        var result = _controller.Edit(movie); //calls the edit method of MovieController with invalid movie object

        //Assert
        var viewResult = Assert.IsType<ViewResult>(result); //ensures its a viewResult
        Assert.NotNull(viewResult);   //checks viewResult is not null ensuring that controller returned a response

        var returnedMovie = Assert.IsType<MovieModel>(viewResult.Model);

        Assert.Equal(movie.MovieName, returnedMovie.MovieName); //ensures invalid model is returned
        Assert.Equal(movie.Genre, returnedMovie.Genre);
        Assert.Equal(movie.Score, returnedMovie.Score);
        Assert.Equal(movie.Studio, returnedMovie.Studio);
        Assert.Equal(movie.ReleaseDate, returnedMovie.ReleaseDate);
    }

    //test if edit method in MovieController correctly redirects to Index when ModelState is valid
    [Fact]
    public void Edit_RedirectsToIndex_WhenModelStateIsValid()
    {
        //Arrange
        var movie = new MovieModel
        {
            MovieName = "Inception",
            Genre = "Action",
            ReleaseDate = DateTime.Now.AddDays(-5), //valid past date
            Studio = "Universal",
            Score = 5
        };

        //Mock of TempData to prevent null reference exception
        _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );


        //Act
        var result = _controller.Edit(movie); //calls the Create method of MovieController with valid movie object

        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result); //ensures its a redirectToActionResult
        Assert.Equal("Index", redirectToActionResult.ActionName); //checks if the action redirects to Index

        //check if movie was actually added to the database
        var addedMovie = _context.Movies.FirstOrDefault(movie => movie.MovieName == "Inception");
        Assert.NotNull(addedMovie); //ensures movie was added to the database
        Assert.Equal("Universal", addedMovie.Studio); //ensures correct movie was added, extra validation 
    }

    //test to check if delete method in MovieController correctly redirects to Index
    [Fact]
    public void Delete_RedirectsToIndex()
    {
        //Arrange
        _context.Database.EnsureDeleted(); // ✅ Clears previous test data
        _context.Database.EnsureCreated(); //recreates the database

        var movie = new MovieModel
        {
            Id = 1,
            MovieName = "Inception",
            Genre = "Action",
            ReleaseDate = DateTime.Now.AddDays(-5), //valid past date
            Studio = "Universal",
            Score = 5
        };

        _context.Movies.Add(movie);
        _context.SaveChanges();

        //Mock of TempData to prevent null reference exception
        _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );


        //Act
        var result = _controller.DeletePost(movie.Id); //calls the delete method of MovieController with valid movie object
        
        //Assert
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result); //ensures its a redirectToActionResult
        Assert.Equal("Index", redirectToActionResult.ActionName); //checks if the action redirects to Index

        //check if movie was actually deleted from the database
        var deletedMovie = _context.Movies.Find(movie.Id);
        Assert.Null(deletedMovie); //ensures movie was deleted from the database
    }


    [Fact]
    public void Index_ReturnsMatchingMovies_WhenQueryIsValid()
    {
        //Arrange: set up test data
        _context.Database.EnsureDeleted(); // ✅ Clears previous test data
        _context.Database.EnsureCreated(); //recreates the database

        var movies = new List<MovieModel>
        {
            new MovieModel {Id=1, MovieName="Inception", Genre="Sci-Fi"},
            new MovieModel {Id=2, MovieName="Interstellar", Genre="Sci-Fi"},
            new MovieModel {Id=3, MovieName="The Dark Knight", Genre="Action"}
        };

        _context.Movies.AddRange(movies); 
        _context.SaveChanges();

        //Act: Call search with query "Inter"
        var result = _controller.Index("Inter") as ViewResult;

        //Assert: Ensure ViewResult is returned
        Assert.NotNull(result);
        

        //check the model only contains "InterStellar"
        var model = Assert.IsAssignableFrom<List<MovieModel>>(result.Model);
        Assert.Single(model);
        Assert.Equal("Interstellar", model.First().MovieName);


    }

}
