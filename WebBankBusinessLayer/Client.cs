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

        public static bool DeleteClientByAccountNumber(string accountNumber, out string errorMessage)
        {
            errorMessage = string.Empty;

            Client client = FindByAccountNumber(accountNumber);
            if (client == null)
            {
                errorMessage = "Client not found or already deleted.";
                return false;
            }

            if (client.Balance > 0)
            {
                errorMessage = $"Cannot delete account. The client still has a balance of {client.Balance}. Account must be cleared first.";
                return false;
            }

            return ClientData.DeleteClientByAccountNumber(accountNumber);
        }

        public static bool Deposit(string accountNumber, decimal amount, out decimal updatedBalance, out string errorMessage)
        {
            updatedBalance = 0;
            errorMessage = string.Empty;

            if (amount < 500 || amount > 50000)
            {
                errorMessage = "Deposit amount must be between 500$ and 50,000$.";
                return false;
            }

            Client client = Client.FindByAccountNumber(accountNumber);
            if (client == null)
            {
                errorMessage = $"Client with Account Number {accountNumber} not found or already inactive.";
                return false;
            }

            if (!ClientData.DepositAmount(accountNumber, amount, out updatedBalance))
            {
                errorMessage = "An internal error occurred while updating the balance in the database.";
                return false;
            }

            return true;
        }

        public static bool Withdraw(string accountNumber, decimal amount, out decimal updatedBalance, out string errorMessage)
        {
            updatedBalance = 0;
            errorMessage = string.Empty;

            if (amount < 50 || amount > 10000)
            {
                errorMessage = "Withdraw amount must be between 50$ and 10,000$.";
                return false;
            }

            Client client = Client.FindByAccountNumber(accountNumber);
            if (client == null)
            {
                errorMessage = $"Client with Account Number {accountNumber} not found.";
                return false;
            }

            if (client.Balance < amount)
            {
                errorMessage = $"Insufficient balance. Your current balance is {client.Balance}$.";
                return false;
            }

            if (!ClientData.WithdrawAmount(accountNumber, amount, out updatedBalance))
            {
                errorMessage = "An internal error occurred during the withdraw process.";
                return false;
            }

            return true;
        }

        public static bool TransferAmount(TransferRequestDTO transferRequest, out decimal senderNewBalance, out string errorMessage)
        {
            senderNewBalance = 0;
            errorMessage = string.Empty;

            if (string.Equals(transferRequest.SenderAccountNumber?.Trim(), transferRequest.ReceiverAccountNumber?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Sender account and receiver account cannot be the same.";
                return false;
            }

            bool result = ClientData.TransferAmount(transferRequest, out senderNewBalance);

            if (!result)
            {
                errorMessage = "Transfer failed. Please check accounts status or balance.";
            }

            return result;
        }

        public static List<TransferLogsDTO> GetAllTransferLogs()
        {
            return ClientData.GetAllTransferLogs();
        }


    }
}

