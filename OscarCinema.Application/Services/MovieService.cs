using AutoMapper;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Entities.Pricing;
using OscarCinema.Domain.Enums.Movie;
using OscarCinema.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Services
{
    public class MovieService : IMovieService
    {
        private readonly IGenericRepository<Movie> _repository;
        private readonly IMapper _mapper;

        public MovieService(IGenericRepository<Movie> movieRepository, IMapper mapper)
        {
            _repository = movieRepository;
            _mapper = mapper;
        }

        public async Task<MovieResponseDTO> CreateAsync(CreateMovieDTO dto)
        {
            var movie = _mapper.Map<Movie>(dto);

            await _repository.AddAsync(movie);

            return _mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<MovieResponseDTO?> GetByIdAsync(int id)
        {
            var movie = await _repository.GetByIdAsync(id);
            return movie == null ? null : _mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllAsync()
        {
            var movies = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<MovieResponseDTO>>(movies ?? Enumerable.Empty<Movie>());
        }

        public async Task<MovieResponseDTO?> UpdateAsync(int id, UpdateMovieDTO dto)
        {
            var existentMovie = await _repository.GetByIdAsync(id);
            if (existentMovie == null)
                return null;

            _mapper.Map(dto, existentMovie);

            await _repository.UpdateAsync(existentMovie);
            return _mapper.Map<MovieResponseDTO>(existentMovie);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _repository.GetByIdAsync(id);
            if (movie == null) return false;

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
