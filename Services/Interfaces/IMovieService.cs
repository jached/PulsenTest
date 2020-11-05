using PulsenTest.Enums;
using PulsenTest.Models;
using System.Threading.Tasks;

namespace PulsenTest.Services.Interfaces
{
    public interface IMovieService
    {
        public Task<Movie> GetMovie(string title, int year, Plot plotLength);
    }
}
