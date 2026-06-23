using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class ClientUpdateDTO
    {
        public int ClientID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First Name must contain letters only.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last Name must contain letters only.")]
        public string LastName { get; set; }

        public string Email { get; set; } = "Unknown";

        [Required(ErrorMessage = "Balance is required.")]
        public decimal? Balance { get; set; }


        public ClientUpdateDTO()
        {
        }

        public ClientUpdateDTO(int clientID, string firstName, string lastName, string email, decimal? balance)
        {
            this.ClientID = clientID;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Balance = balance;
        }
    }
}

