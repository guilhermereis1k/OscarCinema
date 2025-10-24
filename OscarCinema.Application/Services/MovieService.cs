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
    public class MovieService
    {

        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<Movie> CreateAsync(
            string title,
            string description,
            string imageUrl, 
            int duration,
            MovieGenre genre, 
            AgeRating ageRating)
        {
            var movie = new Movie(title, description, imageUrl, duration, genre, ageRating);

            await _movieRepository.CreateAsync(movie);
            return movie;
        }

        public async Task<Movie?> GetByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
                return null;

            return movie;
        }

        public async Task<Movie> UpdateAsync(int id, 
            string title,
            string description,
            string imageUrl,
            int duration,
            MovieGenre genre,
            AgeRating ageRating)
        {
            var existentMovie = await _movieRepository.GetByIdAsync(id);

            if (existentMovie == null) 
                return null;

            existentMovie.Update(
                title,
                description,
                imageUrl,
                duration,
                genre,
                ageRating
            );

            await _movieRepository.UpdateAsync(existentMovie);

            return existentMovie;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

            if (movie == null)
                return false;

            await _movieRepository.DeleteByIdAsync(id);
            return true;
        }
    }
}
