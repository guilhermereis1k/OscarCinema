using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending,    // Pagamento iniciado
        Approved,   // Pagamento confirmado
        Rejected,   // Pagamento negado
        Cancelled,  // Pagamento cancelado pelo usuário
        Refunded    // Pagamento estornado
    }
}
