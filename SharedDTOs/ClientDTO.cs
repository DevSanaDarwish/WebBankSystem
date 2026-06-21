namespace SharedDTOs
{
    public class ClientDTO
    {
        public int ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PinCode { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }

        public ClientDTO()
        {
        }

        public ClientDTO(int clientID, string firstName, string lastName, string email, string pinCode, decimal balance, string accountNumber)
        {
            this.ClientID = clientID;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.PinCode = pinCode;
            this.Balance = balance;
            this.AccountNumber = accountNumber;
        }
    }
}

