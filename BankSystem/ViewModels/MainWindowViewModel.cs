using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BankSystem.Infrastructure.Commands;
using ModelsForBankSystem;
using BankSystem.Views;
using ExceptionLib;

namespace BankSystem.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public MainWindowViewModel()
        {
            _selectedClient = new Client();
            _viewSelectedClient = new Client();
            _clientsList = new ObservableCollection<Client>();
            _selectClientHistoryOfChangesList = new ObservableCollection<ChangesClientArgs>();
            _globalHistoryOfChangesList = new ObservableCollection<ChangesClientArgs>();
            _isConsultant = false;

            #region Команды 

            ICloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);
            ICreateClientCommand = new LambdaCommand(OnCreateClientExecuted, CanCreateClientExecute);
            IEditClientCommand = new LambdaCommand(OnEditClientExecuted, CanEditClientExecute);
            IDeleteClientCommand = new LambdaCommand(OnDeleteClientExecuted, CanDeleteClientExecute);
            IClickOnBankAccount = new LambdaCommand(OnClickOnBankAccountExecuted, CanClickOnBankAccountExecute);
            ISelectManager = new LambdaCommand(OnSelectManagerExecuted, CanSelectManagerExecute);
            ISelectConsultant = new LambdaCommand(OnSelectConsultantExecuted, CanSelectConsultantExecute);
            #endregion
        }

        private bool _isConsultant;
        public bool IsConsultant { get => _isConsultant; set => Set(ref _isConsultant, value); }

        private BankAccountWindow _bankAccountWindow;

        private Client _viewSelectedClient;
        public Client ViewSelectedClient { get => _viewSelectedClient; set => Set(ref _viewSelectedClient, value); }

        private Client _selectedClient;
        public Client SelectedClient
        {
            get => _selectedClient;
            set
            {
                Set(ref _selectedClient, value);
                if(ViewSelectedClient != null && SelectedClient != null)
                {
                    ViewSelectedClient = SelectedClient.Clone() as Client;
                    SelectClientHistoryOfChangesList = SelectedClient.HistoryOfChangesList;
                }
            }
        }

        private ObservableCollection<Client> _clientsList;
        public ObservableCollection<Client> ClientsList { get => _clientsList; set => Set(ref _clientsList, value); }

        private ObservableCollection<ChangesClientArgs> _selectClientHistoryOfChangesList;
        public ObservableCollection<ChangesClientArgs> SelectClientHistoryOfChangesList { get => _selectClientHistoryOfChangesList; set => Set(ref _selectClientHistoryOfChangesList, value); }

        private ObservableCollection<ChangesClientArgs> _globalHistoryOfChangesList;
        public ObservableCollection<ChangesClientArgs> GlobalHistoryOfChangesList { get => _globalHistoryOfChangesList; set => Set(ref _globalHistoryOfChangesList, value); }


        #region Команды

        #region CloseApplicationCommand

        public ICommand ICloseApplicationCommand { get; }
        private bool CanCloseApplicationCommandExecute(object p) => true;

        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }

        #endregion

        public ICommand ISelectManager { get; }

        private bool CanSelectManagerExecute(object p) => IsConsultant;

        private void OnSelectManagerExecuted(object p)
        {
            if (_isConsultant == true)
            {
                _isConsultant = false;
            }
        }

        public ICommand ISelectConsultant{ get; }

        private bool CanSelectConsultantExecute(object p) => !IsConsultant;

        private void OnSelectConsultantExecuted(object p)
        {
            if (_isConsultant == false)
            {
                _isConsultant = true;
            }
        }

        public ICommand ICreateClientCommand { get; }

        private bool CanCreateClientExecute(object p) => !_isConsultant;

        private void OnCreateClientExecuted(object p)
        {
            try
            {
                if (ViewSelectedClient.PhoneNumber == 0)
                {
                    throw new IncorrectDataClientException("Ошибка", 1);
                }
                else if (ViewSelectedClient.PassportNumber == 0)
                {
                    throw new IncorrectDataClientException("Ошибка", 2);
                }
                Client newClient = new Client
                {
                    Surname = ViewSelectedClient.Surname,
                    Name = ViewSelectedClient.Name,
                    Patronymic = ViewSelectedClient.Patronymic,
                    PhoneNumber = ViewSelectedClient.PhoneNumber,
                    PassportNumber = ViewSelectedClient.PassportNumber,
                };
                var args = new ChangesClientArgs(ChangesClientArgs.TypeChanges.Create, $"{newClient.Surname} {newClient.Name} {newClient.Patronymic}");
                newClient.Changes += AddNewChangesInHistoryOfChangesList;
                newClient.InvokeEvent(args);
                ClientsList.Add(newClient);
                SelectedClient = ClientsList.Last();
            }
            catch (IncorrectDataClientException e) when (e.CodeError == 1)
            {
                MessageBox.Show("Не заполено поле номера телефона!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IncorrectDataClientException e) when (e.CodeError == 2)
            {
                MessageBox.Show("Не заполено поле номера паспорта!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Не предусмотренная ошибка {e.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ICommand IEditClientCommand { get; }

        private bool CanEditClientExecute(object p) => p is Client client && ClientsList.Contains(client) && !_isConsultant;

        private void OnEditClientExecuted(object p)
        {
            SelectedClient.ChangeInformation(ViewSelectedClient);
            ViewSelectedClient = SelectedClient;
            ObservableCollection<Client> cL = ClientsList;
            ClientsList = null;
            ClientsList = cL;
            SelectedClient = ViewSelectedClient;
            var args = new ChangesClientArgs(ChangesClientArgs.TypeChanges.Edit, $"{SelectedClient.Surname} {SelectedClient.Name} {SelectedClient.Patronymic}");
            SelectedClient.InvokeEvent(args);
            OnPropertyChanged(nameof(SelectedClient.Name));
            OnPropertyChanged(nameof(ViewSelectedClient));
            OnPropertyChanged(nameof(ClientsList));
        }

        private void ClientsList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public ICommand IDeleteClientCommand { get; }

        private bool CanDeleteClientExecute(object p) => p is Client client && ClientsList.Contains(client) && !_isConsultant;

        private void OnDeleteClientExecuted(object p)
        {
            var args = new ChangesClientArgs(ChangesClientArgs.TypeChanges.Delete, $"{SelectedClient.Surname} {SelectedClient.Name} {SelectedClient.Patronymic}");
            SelectedClient.InvokeEvent(args);
            ClientsList.Remove(SelectedClient);
        }

        public ICommand IClickOnBankAccount { get; }

        private bool CanClickOnBankAccountExecute(object p) => p is Client client && ClientsList.Contains(client) && !_isConsultant;

        private void OnClickOnBankAccountExecuted(object p)
        {
            _bankAccountWindow = new BankAccountWindow();
            BankAccountViewModel viewModel = new BankAccountViewModel();
            viewModel.SelectedClient = SelectedClient;
            viewModel.ClientsList = ClientsList;
            viewModel.SetViewClientName();
            _bankAccountWindow.DataContext = viewModel;
            _bankAccountWindow.Show();
        }

        #endregion

        #region Вспомогательные методы

        private void AddNewChangesInHistoryOfChangesList(object sender, ChangesClientArgs e)
        {
            GlobalHistoryOfChangesList.Add(e);
        }

        #endregion

        #region Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _title = "Банковская система";
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }
        #endregion
    }
}
