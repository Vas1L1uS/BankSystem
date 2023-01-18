using BankSystem.Infrastructure.Commands;
using BankSystem.Models.BankAccounts;
using BankSystem.Models.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BankSystem.Models;

namespace BankSystem.ViewModels
{
    internal class BankAccountViewModel : ViewModel
    {
        public BankAccountViewModel()
        {
            _gridMain = new Grid();
            _gridMain.Visibility = Visibility.Visible;
            _gridTransfer = new Grid();
            _gridTransfer.Visibility = Visibility.Collapsed;
            _gridRefill = new Grid();
            _gridRefill.Visibility = Visibility.Collapsed;

            _beneficiaryClient = new Client();

            ICreateBankAccount = new LambdaCommand(OnCreateBankAccountExecuted, CanCreateBankAccountExecute);
            IDeleteBankAccount = new LambdaCommand(OnDeleteBankAccountExecuted, CanDeleteBankAccountExecute);
            IToBankAccoutPage = new LambdaCommand(OnToBankAccoutPageExecuted, CanToBankAccoutPageExecute);
            IToTransferPage = new LambdaCommand(OnToTransferPageExecuted, CanToTransferPageExecute);
            IToRefillPage = new LambdaCommand(OnToRefillPageExecuted, CanToRefillPageExecute);
            ITransfer = new LambdaCommand(OnTransferExecuted, CanTransferExecute);
            ITopUp = new LambdaCommand(OnTopUpExecuted,CanTopUpExecute);
        }

        private int _transferAmount;
        public int TransferAmount
        {
            get => _transferAmount;
            set => Set(ref _transferAmount, value);
        }

        private int _topUpAmount;
        public int TopUpAmount
        {
            get => _topUpAmount;
            set => Set(ref _topUpAmount, value);
        }

        private BankAccount _selectedBankAccount;
        public BankAccount SelectedBankAccount
        {
            get => _selectedBankAccount;
            set => Set(ref _selectedBankAccount, value);
        }

        private BankAccount _beneficiaryAccount;
        public BankAccount BeneficiaryAccount
        {
            get => _beneficiaryAccount;
            set => Set(ref _beneficiaryAccount, value);
        }

        private ObservableCollection<Client> _clientsList;
        public ObservableCollection<Client> ClientsList { get => _clientsList; set => Set(ref _clientsList, value); }

        private string _selectedClientName;
        public string SelectedClientName
        {
            get => _selectedClientName;
            set => Set(ref _selectedClientName, value);
        }
        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set => Set(ref _selectedClient, value);
        }

        private Client _beneficiaryClient;
        public Client BeneficiaryClient
        {
            get => _beneficiaryClient;
            set => Set(ref _beneficiaryClient, value);
        }

        private UIElement _gridMain;
        public UIElement GridMain
        {
            get => _gridMain;
            set => Set(ref _gridMain, value);
        }

        private UIElement _gridTransfer;
        public UIElement GridTransfer
        {
            get => _gridTransfer;
            set => Set(ref _gridTransfer, value);
        }

        private UIElement _gridRefill;
        public UIElement GridRefill
        {
            get => _gridRefill;
            set => Set(ref _gridRefill, value);
        }

        public void SetViewClientName()
        {
            _selectedClientName = $"{_selectedClient.Surname} {_selectedClient.Name} {SelectedClient.Patronymic}";
        }

        #region Команды

        public ICommand ICreateBankAccount { get; }
        private bool CanCreateBankAccountExecute(object p) => true;

        private void OnCreateBankAccountExecuted(object p)
        {
            SelectedClient.BankAccountsList.Add(new BankAccount());
            var args = new ChangesClientArgs(ChangesClientArgs.TypeChanges.OpenAccount, $"{SelectedClient.Surname} {SelectedClient.Name} {SelectedClient.Patronymic} Открыт счет {SelectedClient.BankAccountsList.Last().ID}");
            SelectedClient.InvokeEvent(args);
        }

        public ICommand IDeleteBankAccount { get; }
        private bool CanDeleteBankAccountExecute(object p) => p is BankAccount bankAccount && SelectedClient.BankAccountsList.Contains(SelectedBankAccount);

        private void OnDeleteBankAccountExecuted(object p)
        {
            var args = new ChangesClientArgs(ChangesClientArgs.TypeChanges.CloseAccount, $"{SelectedClient.Surname} {SelectedClient.Name} {SelectedClient.Patronymic} Закрыт счет {SelectedBankAccount.ID}");
            SelectedClient.InvokeEvent(args);
            SelectedClient.BankAccountsList.Remove(SelectedBankAccount);
        }

        public ICommand IToBankAccoutPage { get; }
        private bool CanToBankAccoutPageExecute(object p) => true;

        private void OnToBankAccoutPageExecuted(object p)
        {
            GridMain.Visibility = Visibility.Visible;
            GridTransfer.Visibility = Visibility.Collapsed;
            GridRefill.Visibility = Visibility.Collapsed;
        }

        public ICommand IToTransferPage { get; }
        private bool CanToTransferPageExecute(object p) => p is BankAccount bankAccount && SelectedClient.BankAccountsList.Contains(SelectedBankAccount);

        private void OnToTransferPageExecuted(object p)
        {
            GridMain.Visibility = Visibility.Collapsed;
            GridTransfer.Visibility = Visibility.Visible;
            GridRefill.Visibility = Visibility.Collapsed;
        }

        public ICommand IToRefillPage { get; }
        private bool CanToRefillPageExecute(object p) => p is BankAccount bankAccount && SelectedClient.BankAccountsList.Contains(SelectedBankAccount);

        private void OnToRefillPageExecuted(object p)
        {
            GridMain.Visibility = Visibility.Collapsed;
            GridTransfer.Visibility = Visibility.Collapsed;
            GridRefill.Visibility = Visibility.Visible;
        }

        public ICommand ITransfer { get; }
        private bool CanTransferExecute(object p) => p is BankAccount bankAccount && p != SelectedBankAccount && BeneficiaryClient.BankAccountsList.Contains(BeneficiaryAccount);

        private void OnTransferExecuted(object p)
        {
            if (SelectedBankAccount.Transfer(BeneficiaryAccount, TransferAmount))
            {
                if (TransferAmount >= 10)
                {
                    MessageBox.Show($"Перевод успешно осуществлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    OnPropertyChanged(nameof(SelectedBankAccount));
                    OnPropertyChanged(nameof(BeneficiaryAccount));
                    var args = new ChangesClientArgs(ChangesClientArgs.TypeChanges.Transfer, $" Счет {SelectedBankAccount.ID} перевел {TransferAmount} на счет {BeneficiaryAccount.ID}");
                    SelectedClient.InvokeEvent(args);
                }
                else
                {
                    MessageBox.Show($"Перевод должен быть не меньше 10!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show($"Недостаточно средств!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ICommand ITopUp { get; }
        private bool CanTopUpExecute(object p) => true;

        private void OnTopUpExecuted(object p)
        {
            SelectedBankAccount.AddMoney(TopUpAmount);
            MessageBox.Show($"Счет успешно пополнен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            GridMain.Visibility = Visibility.Visible;
            GridRefill.Visibility = Visibility.Collapsed;
            OnPropertyChanged(nameof(SelectedBankAccount));
            var args = new ChangesClientArgs(ChangesClientArgs.TypeChanges.Refill, $" Счет {SelectedBankAccount.ID} был пополнен на {TopUpAmount}");
            SelectedClient.InvokeEvent(args);
        }

        #endregion

        #region Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _title = $"Управление банковскими счетами";
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        #endregion
    }
}
