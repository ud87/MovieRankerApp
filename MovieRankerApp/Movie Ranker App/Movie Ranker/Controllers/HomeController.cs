using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Movie_Ranker.Models;

namespace Movie_Ranker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); //added "Error" to new ErrorViewModel this tells ASP.NET Core to look for a view named Error in the Views/Shared folder
        }
    }
}
