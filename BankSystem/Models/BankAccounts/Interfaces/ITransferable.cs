using BankSystem.Models.BankAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Models.Interfaces
{
    interface ITransferable
    {
        bool Transfer(BankAccount bankAccount, int transferAmount);
    }
}
