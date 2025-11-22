using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarCinema.Domain.Enums.Ticket
{
    public enum PaymentMethod
    {
        Cash,           // Dinheiro
        CreditCard,     // Cartão de crédito
        DebitCard,      // Cartão de débito
        Pix,            // Pix 
        BankSlip,       // Boleto bancário
        DigitalWallet,  // Carteiras digitais
    }
}
