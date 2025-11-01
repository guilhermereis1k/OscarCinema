using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Enums.Movie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponseDTO> CreateAsync(CreateMovieDTO dto);
        Task<MovieResponseDTO?> GetByIdAsync(int id);
        Task<IEnumerable<MovieResponseDTO>> GetAllAsync();
        Task<MovieResponseDTO?> UpdateAsync(int id, UpdateMovieDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
