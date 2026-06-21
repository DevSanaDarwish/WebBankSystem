using System.Data;
using WebBankDataAccessLayer;
using SharedDTOs;

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

        public static List<ClientDTO> GetAllClients()
        {
            return ClientData.GetAllClients();
        }
    }
}

