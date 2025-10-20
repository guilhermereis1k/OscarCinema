using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Enums
{
    public enum TicketType
    {
        Full, // Inteira
        Half, // Meia
        StudentHalf // Meia de estudante (necessita validação de documento de estudante)
    }
}
