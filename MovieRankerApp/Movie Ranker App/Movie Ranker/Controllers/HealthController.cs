using Microsoft.AspNetCore.Mvc;

namespace Movie_Ranker.Controllers
{
    [ApiController]     //sets the controller to be an API controller
    [Route("api/[controller]")]   //sets the route to /api/health
    public class HealthController : ControllerBase //inherits from ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Server is alive!");
        }
    }
}
