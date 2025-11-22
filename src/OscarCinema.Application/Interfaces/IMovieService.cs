using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.DTOs.Pagination;
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
        Task<MovieResponse> CreateAsync(CreateMovie dto);
        Task<MovieResponse?> GetByIdAsync(int id);
        Task<PaginationResult<MovieResponse>> GetAllAsync(PaginationQuery query);
        Task<MovieResponse?> UpdateAsync(int id, UpdateMovie dto);
        Task<bool> DeleteAsync(int id);
    }
}
