using OscarCinema.Domain.Entities;
using OscarCinema.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(int userId, string email, string userName);
    }
}
