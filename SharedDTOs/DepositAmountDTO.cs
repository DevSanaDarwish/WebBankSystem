using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class DepositAmountDTO
    {
        [Required(ErrorMessage = "Deposit amount is required.")]
        [Range(500.00, 50000.00, ErrorMessage = "Deposit amount must be between 500$ and 50,000$.")]
        public decimal Amount { get; set; }
    }
}
