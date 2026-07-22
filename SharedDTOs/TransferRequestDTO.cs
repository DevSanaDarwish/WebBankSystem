using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class TransferRequestDTO
    {
        [Required(ErrorMessage = "Sender account number is required.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Account Number must be exactly 8 characters.")]
        public string SenderAccountNumber { get; set; }

        [Required(ErrorMessage = "Receiver account number is required.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Account Number must be exactly 8 characters.")]
        public string ReceiverAccountNumber { get; set; }

        [Required(ErrorMessage = "Transfer amount is required and cannot be left empty.")]
        [Range(100.00, 20000.00, ErrorMessage = "Transfer amount must be between 100$ and 20,000$.")]
        public decimal? TransferAmount { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int CreatedByUserID { get; set; }
    }
}
