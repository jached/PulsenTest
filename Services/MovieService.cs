using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PulsenTest.Enums;
using PulsenTest.Models;
using PulsenTest.Services.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PulsenTest.Services
{
    public class MovieService : IMovieService
    {
        private readonly IHttpClientFactory HttpClientFactory;
        private readonly ILogger<MovieService> Logger;
        private readonly IConfiguration Configuration;

        public MovieService(IHttpClientFactory httpClientFactory, ILogger<MovieService> logger, IConfiguration configuration)
        {
            HttpClientFactory = httpClientFactory;
            Logger = logger;
            Configuration = configuration;
        }
        public async Task<Movie> GetMovie(string title, int year, Plot plotLength)
        {
            var httpClient = HttpClientFactory.CreateClient("MovieSender");

            var titleParam = $"t={title}";
            var yearParam = year == -1 ? string.Empty : $"&y={year}";
            var plotParam = $"&plot={plotLength}";
            var apiKey = Configuration["OmdbApiKey"];
            var uri = $"http://www.omdbapi.com/?{titleParam}{yearParam}{plotParam}&apikey={apiKey}";

            var response = await httpClient.GetAsync(uri).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (json.ToLower().Contains("movie not found")) return null;

            try
            {
                return JsonConvert.DeserializeObject<Movie>(json);
            }
            catch (Exception e)
            {
                Logger.LogError("Failed to deserialize string", e);
                throw e;
            }
        }
    }
}
