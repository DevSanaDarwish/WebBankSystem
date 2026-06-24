using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class UpdatePinCodeDTO
    {
        [Required(ErrorMessage = "New Pin Code is required.")]
        [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Pin Code must be exactly 4 digits.")]
        public string NewPinCode { get; set; }
    }
}
