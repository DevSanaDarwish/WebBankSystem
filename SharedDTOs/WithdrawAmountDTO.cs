using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class WithdrawAmountDTO
    {
        [Required(ErrorMessage = "Withdraw amount is required.")]
        [Range(50.00, 10000.00, ErrorMessage = "Withdraw amount must be between 50$ and 10,000$.")]
        public decimal Amount { get; set; }
    }
}
