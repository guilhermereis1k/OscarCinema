using AutoMapper;
using OscarCinema.Application.DTOs.Movie;
using OscarCinema.Application.Interfaces;
using OscarCinema.Domain.Entities;
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
        private readonly IMovieRepository _movieRepository;
        private readonly IMapper _mapper;

        public MovieService(IMovieRepository movieRepository, IMapper mapper)
        {
            _movieRepository = movieRepository;
            _mapper = mapper;
        }

        public async Task<MovieResponseDTO> CreateAsync(CreateMovieDTO dto)
        {
            var movie = _mapper.Map<Movie>(dto);

            await _movieRepository.CreateAsync(movie);

            return _mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<MovieResponseDTO?> GetByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            return movie == null ? null : _mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<IEnumerable<MovieResponseDTO>> GetAllAsync()
        {
            var movies = await _movieRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<MovieResponseDTO>>(movies ?? Enumerable.Empty<Movie>());
        }

        public async Task<MovieResponseDTO?> UpdateAsync(int id, UpdateMovieDTO dto)
        {
            var existentMovie = await _movieRepository.GetByIdAsync(id);
            if (existentMovie == null)
                return null;

            _mapper.Map(dto, existentMovie);

            await _movieRepository.UpdateAsync(existentMovie);
            return _mapper.Map<MovieResponseDTO>(existentMovie);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return false;

            await _movieRepository.DeleteByIdAsync(id);
            return true;
        }
    }
}
