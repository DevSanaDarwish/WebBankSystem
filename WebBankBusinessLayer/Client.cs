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
                    this.Balance,        
                    this.AccountNumber, 
                    this.PinCode
                );
            }
        }

        public ClientUpdateDTO UpdateCDTO
        {
            get
            {
                return new ClientUpdateDTO(
                    this.ClientID,
                    this.PersonInfo.FirstName,
                    this.PersonInfo.LastName,
                    this.PersonInfo.Email,
                    this.Balance
                );
            }
        }

        public Client(ClientDTO CDTO, enMode cMode = enMode.AddNew)
        {
            this.ClientID = CDTO.ClientID;
            this.PinCode = CDTO.PinCode;
            this.Balance = CDTO.Balance ?? 0;
            this.AccountNumber = CDTO.AccountNumber;

            this.PersonInfo = new Person();
            this.PersonInfo.FirstName = CDTO.FirstName;
            this.PersonInfo.LastName = CDTO.LastName;
            this.PersonInfo.Email = CDTO.Email;

            Mode = cMode;
        }

        public static string MaskAccountNumber(string accountNumber)
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
                client.AccountNumber = MaskAccountNumber(client.AccountNumber);
                client.PinCode = "****";
            }

            return rawList;
        }

        private bool AddNewClient()
        {
            this.ClientID = ClientData.AddNewClient(CDTO);

            return (this.ClientID != -1);
        }

        private bool UpdateClient()
        {
            return ClientData.UpdateClient(UpdateCDTO);
        }

        public static bool UpdatePinCode(string accountNumber, string newPinCode)
        {
            return ClientData.UpdateClientPinCode(accountNumber, newPinCode);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (AddNewClient())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return UpdateClient();

            }

            return false;
        }

        public static bool IsAccountNumberExist(string accountNumber)
        {
            return ClientData.IsAccountNumberExist(accountNumber);
        }

        public static bool IsValidClient(ClientUpdateDTO clientDTO, out string errorMessage)
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

            //if (IsAccountNumberExist(clientDTO.AccountNumber))
            //{
            //    errorMessage = "Account Number already exists. Please choose a unique number.";
            //    return false;
            //}     

            return true;
        }

        public static Client FindByClientId(int clientId)
        {
            ClientDTO clientDTO = ClientData.GetClientByID(clientId);

            if (clientDTO != null)
            {
                clientDTO.PinCode = "****";
                clientDTO.AccountNumber = MaskAccountNumber(clientDTO.AccountNumber);

                return new Client(clientDTO, enMode.Update);
            }

            return null;
        }

        public static Client FindByAccountNumber(string accountNumber)
        {
            ClientDTO clientDTO = ClientData.GetClientByAccountNumber(accountNumber);

            if (clientDTO != null)
            {
                return new Client(clientDTO, enMode.Update); 
            }

            return null;
        }
    }
}

