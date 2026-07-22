using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedDTOs
{
    public class TransferLogsDTO
    {
        public int TransferLogID { get; set; }

        public DateTime Date { get; set; }

        public string SourceAcc { get; set; }       

        public string DestinationAcc { get; set; }   

        public decimal Amount { get; set; }

        public decimal SourceBalance { get; set; }

        public decimal DestinationBalance { get; set; }

        public string Username { get; set; }

        public TransferLogsDTO() { }

        public TransferLogsDTO(int transferLogID, DateTime date, string sourceAcc, string destinationAcc, decimal amount, decimal sourceBalance, decimal destinationBalance, string username)
        {
            TransferLogID = transferLogID;
            Date = date;
            SourceAcc = sourceAcc;
            DestinationAcc = destinationAcc;
            Amount = amount;
            SourceBalance = sourceBalance;
            DestinationBalance = destinationBalance;
            Username = username;
        }
    }
}
