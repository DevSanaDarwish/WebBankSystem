using System.ComponentModel.DataAnnotations;

namespace SharedDTOs
{
    public class ClientDTO : ClientUpdateDTO
    {
        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Account Number must be exactly 8 characters.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Pin Code is required.")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin Code must be exactly 4 digits.")]
        public string PinCode { get; set; }

        public ClientDTO()
        {
        }

        public ClientDTO(int clientID, string firstName, string lastName, string email, decimal? balance, string accountNumber, string pinCode)
            : base(clientID, firstName, lastName, email, balance)
        {
            this.AccountNumber = accountNumber;
            this.PinCode = pinCode;
        }
    }
}

