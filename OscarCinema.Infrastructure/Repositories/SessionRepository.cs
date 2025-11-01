﻿using Microsoft.EntityFrameworkCore;
using OscarCinema.Domain.Entities;
using OscarCinema.Domain.Interfaces;
using OscarCinema.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Infrastructure.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(OscarCinemaContext context) : base(context) { }

        public async Task<IEnumerable<Session>> GetAllByMovieId(int movieId)
        {
            return await _context.Sessions
                .Where(s =>  s.MovieId == movieId)
                .ToListAsync();
        }
    }
}
