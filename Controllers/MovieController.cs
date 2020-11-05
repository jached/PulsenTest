using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PulsenTest.Enums;
using PulsenTest.Services.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace PulsenTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService MovieService;
        private readonly ILogger<MovieController> Logger;

        public MovieController(IMovieService movieService, ILogger<MovieController> logger)
        {
            MovieService = movieService;
            Logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> GetMovie(string movieTitle, int? movieYear = -1, Plot? moviePlot = Plot.SHORT)
        {
            try
            {
                var movie = await MovieService.GetMovie(movieTitle, (int)movieYear, (Plot)moviePlot);
                if (movie != null)
                {
                    Logger.LogInformation("Successfully retrieved movie", movie.Title);
                    return Ok(movie);
                }

                Logger.LogInformation("Failed to find movie", movieTitle);
                return BadRequest(new { Error = $"Unable to find movie: {movieTitle}" });
            }
            catch (Exception e)
            {
                Logger.LogError(nameof(MovieController), e);
                return StatusCode((int)HttpStatusCode.InternalServerError, e);
            }
        }
    }
}
