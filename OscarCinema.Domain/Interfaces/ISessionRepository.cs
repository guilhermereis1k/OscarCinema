﻿using OscarCinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllByMovieId( int movieId);
    }
}
