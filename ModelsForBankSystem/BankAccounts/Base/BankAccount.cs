using ModelsForBankSystem.Interfaces;

namespace ModelsForBankSystem.BankAccounts
{
    public class BankAccount : ITransferable
    {
        public bool AccountOpen
        {
            get { return _accountOpen; }
            set { if (_accountOpen != value) _accountOpen = value; }
        }
        public int Money
        {
            get { return _money; }
            set { _money = value; }
        }
        public ulong ID { get; set; }
        public Client MyClient { get; protected set; }

        static private ulong s_maxId;
        static private ulong NextId()
        {
            s_maxId++;
            return s_maxId;
        }
        private int _money;
        private bool _accountOpen;

        public void AddMoney(int addAmount)
        {
            this._money += addAmount;
        }

        static BankAccount()
        {
            s_maxId = 1000000000000000; // 16 знаков
        }

        public BankAccount()
        {
            _money = 0;
            _accountOpen = true;
            ID = NextId();
        }

        public bool Transfer(BankAccount bankAccount, int transferAmount)
        {
            if (this.Money >= transferAmount && this.AccountOpen && bankAccount.AccountOpen)
            {
                this.AddMoney(-transferAmount);
                bankAccount.AddMoney(transferAmount);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
