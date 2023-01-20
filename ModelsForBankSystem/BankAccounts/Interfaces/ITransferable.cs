using ModelsForBankSystem.BankAccounts;

namespace ModelsForBankSystem.Interfaces
{
    public interface ITransferable
    {
        bool Transfer(BankAccount bankAccount, int transferAmount);
    }
}