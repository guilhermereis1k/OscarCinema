using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie> CreateAsync(Movie movie);
        Task<Movie> UpdateAsync(Movie movie);
        Task<Movie?> GetByIdAsync(int id);
        Task<bool> DeleteByIdAsync(int id);
        Task<IEnumerable<Movie>> GetAllAsync();
    }
}
