using System.Data;
using WebBankDataAccessLayer;
using SharedDTOs;
using System.Text.RegularExpressions;

namespace WebBankBusinessLayer
{
    public class Client
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int ClientID { get; set; }
        public string PinCode { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }
        public Person PersonInfo { get; set; }

        public ClientDTO CDTO
        {
            get
            {
                return new ClientDTO(
                    this.ClientID,
                    this.PersonInfo.FirstName,
                    this.PersonInfo.LastName,
                    this.PersonInfo.Email,
                    this.PinCode,
                    this.Balance,
                    this.AccountNumber
                );
            }
        }

        public Client(ClientDTO CDTO, enMode cMode = enMode.AddNew)
        {
            this.ClientID = CDTO.ClientID;
            this.PinCode = CDTO.PinCode;
            this.Balance = CDTO.Balance;
            this.AccountNumber = CDTO.AccountNumber;

            this.PersonInfo = new Person();
            this.PersonInfo.FirstName = CDTO.FirstName;
            this.PersonInfo.LastName = CDTO.LastName;
            this.PersonInfo.Email = CDTO.Email;

            Mode = cMode;
        }

        private static string _MaskAccountNumber(string accountNumber)
        {
            return accountNumber.Substring(0, 2) + "****" + accountNumber.Substring(accountNumber.Length - 2);
        }

        public static List<ClientDTO> GetAllClients()
        {
            List<ClientDTO> rawList = ClientData.GetAllClients();

            if (rawList == null)
                return null;

            foreach (var client in rawList)
            {
                client.AccountNumber = _MaskAccountNumber(client.AccountNumber);
            }

            return rawList;
        }

        private bool _AddNewStudent()
        {
            this.ClientID = ClientData.AddNewClient(CDTO);

            return (this.ClientID != -1);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewStudent())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                //case enMode.Update:

                    //return _UpdateStudent();

            }

            return false;
        }

        public static bool IsAccountNumberExist(string accountNumber)
        {
            return ClientData.IsAccountNumberExist(accountNumber);
        }

        public static bool IsValidClient(ClientDTO clientDTO, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (clientDTO == null)
            {
                errorMessage = "Client data cannot be null.";
                return false;
            }

            if (clientDTO.Balance < 0)
            {
                errorMessage = "Balance cannot be a negative number.";
                return false;
            }

            if (IsAccountNumberExist(clientDTO.AccountNumber))
            {
                errorMessage = "Account Number already exists. Please choose a unique number.";
                return false;
            }

            return true;
        }
    }
}

