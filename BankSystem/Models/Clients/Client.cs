using BankSystem.Models.BankAccounts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Xml.Linq;

namespace BankSystem.Models.Clients
{
    class Client : IComparable<Client>, ICloneable
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string Patronymic { get; set; }
        public ulong PhoneNumber { get; set; }
        public ulong PassportNumber { get; set; }
        public DateTime DateCreate { get; set; }
        public ObservableCollection<ChangesClientArgs> HistoryOfChangesList { get; private set; }
        public ObservableCollection<BankAccount> BankAccountsList { get; set; }


        public Client(string surname, string name, string patronymic, ulong phoneNumber, ulong passportNumber)
        {

            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            PhoneNumber = phoneNumber;
            PassportNumber = passportNumber;
            DateCreate = DateTime.Now;
            HistoryOfChangesList = new ObservableCollection<ChangesClientArgs>();
            this._changes += this.AddNewChangesInHistoryOfChangesList;
            BankAccountsList = new ObservableCollection<BankAccount>();
        }

        public Client()
        {
            DateCreate = DateTime.Now;
            HistoryOfChangesList = new ObservableCollection<ChangesClientArgs>();
            this._changes += this.AddNewChangesInHistoryOfChangesList;
            BankAccountsList = new ObservableCollection<BankAccount>();
        }

        public void InvokeEvent(ChangesClientArgs args)
        {
            this._changes?.Invoke(this, args);
        }

        public int CompareTo(Client other)
        {
            if (this.Surname.CompareTo(other.Surname) == 0)
            {
                if (this.Name.CompareTo(other.Name) == 0)
                {
                    return this.Patronymic.CompareTo(other.Patronymic);
                }
                else
                {
                    return this.Name.CompareTo(other.Name);
                }
            }
            else
            {
                return this.Surname.CompareTo(other.Surname);
            }
        }

        public void ChangeInformation(Client client)
        {
            this.Name = client.Name;
            this.Surname = client.Surname;
            this.Patronymic = client.Patronymic;
            this.PhoneNumber = client.PhoneNumber;
            this.PassportNumber = client.PassportNumber;
        }

        private void AddNewChangesInHistoryOfChangesList(object sender, ChangesClientArgs e)
        {
            HistoryOfChangesList.Add(e);
        }

        private event Action<object, ChangesClientArgs> _changes;

        public event Action<object, ChangesClientArgs> Changes
        {
            add
            {
                _changes += value;
            }
            remove
            {
                _changes -= value;
            }
        }


        public object Clone()
        {
            var newClient = new Client();
            newClient.Surname = this.Surname;
            newClient.Name = this.Name;
            newClient.Patronymic = this.Patronymic;
            newClient.PhoneNumber = this.PhoneNumber;
            newClient.PassportNumber = this.PassportNumber;
            newClient.DateCreate = this.DateCreate;
            newClient.HistoryOfChangesList = this.HistoryOfChangesList;
            newClient.BankAccountsList = this.BankAccountsList;
            return newClient;
        }
    }
}
