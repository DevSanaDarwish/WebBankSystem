using System.ComponentModel.DataAnnotations;

namespace SharedDTOs
{
    public class ClientDTO
    {
        public int ClientID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First Name must contain letters only.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name must contain letters only.")]
        public string LastName { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Pin Code is required.")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin Code must be exactly 4 digits.")]
        public string PinCode { get; set; }

        [Required(ErrorMessage = "Balance is required.")]
        public decimal? Balance { get; set; }

        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Account Number must be exactly 8 characters.")]
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

