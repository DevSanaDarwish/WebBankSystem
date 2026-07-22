using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebBankBusinessLayer; 
using SharedDTOs;          

namespace WebBankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        [HttpGet("all", Name = "GetAllClients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public ActionResult<IEnumerable<ClientDTO>> GetAllClients()
        {
            List<ClientDTO> clientsList = Client.GetAllClients();

            if (clientsList == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Database connection failed.");
            }

            if (clientsList.Count == 0)
            {
                return NotFound("No clients found in the database.");
            }

            return Ok(clientsList);
        }

        [HttpPost(Name = "AddClient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ClientDTO> AddNewClient([FromBody] ClientDTO newClientDTO)
        {
            if (!Client.IsValidClient(newClientDTO, out string validationError))
            {
                return BadRequest(validationError);
            }

            if (Client.IsAccountNumberExist(newClientDTO.AccountNumber))
            {
                return BadRequest("Account Number already exists. Please choose a unique number.");
            }

            Client client = new Client
                (
                new ClientDTO
                (
                    newClientDTO.ClientID,
                    newClientDTO.FirstName,
                    newClientDTO.LastName,
                    newClientDTO.Email,
                    newClientDTO.Balance,       
                    newClientDTO.AccountNumber, 
                    newClientDTO.PinCode
                )
            );

            if (!client.Save())
            {
                return StatusCode(500, "An internal server error occurred while adding the new client.");
            }

            ClientDTO returnDTO = client.CDTO;
            returnDTO.PinCode = "****";
            returnDTO.AccountNumber = Client.MaskAccountNumber(returnDTO.AccountNumber);

            return CreatedAtRoute("GetClientById", new { id = returnDTO.ClientID }, returnDTO);


            //newClientDTO.ClientID = client.ClientID;

            //return CreatedAtRoute("GetClientById", new { id = newClientDTO.ClientID }, newClientDTO);
        }


        [HttpGet("id{id}", Name = "GetClientById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClientDTO> GetClientById([FromRoute] int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }

            Client client = Client.FindByClientId(id);

            if (client == null)
            {
                return NotFound($"Client with ID {id} not found.");
            }

            ClientDTO CDTO = client.CDTO;

            return Ok(CDTO);
        }

        [HttpPut("{accountNumber}", Name = "UpdateClientByAccountNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClientDTO> UpdateClientByAccountNumber([FromRoute] string accountNumber, ClientUpdateDTO updatedClientDTO)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 8)
            {
                return BadRequest("Invalid Account Number format. It must be exactly 8 characters long.");
            }

            if (!Client.IsValidClient(updatedClientDTO, out string validationError))
            {
                return BadRequest(validationError);
            }

            Client client = Client.FindByAccountNumber(accountNumber);

            if (client == null)
            {
                return NotFound($"Client with Account Number {accountNumber} not found.");
            }

            client.PersonInfo.FirstName = updatedClientDTO.FirstName;
            client.PersonInfo.LastName = updatedClientDTO.LastName;
            client.PersonInfo.Email = updatedClientDTO.Email;
            client.Balance = updatedClientDTO.Balance.Value;

            client.Save();

            return Ok(client.CDTO);
        }

        [HttpPatch("update-pincode/{accountNumber}", Name = "UpdatePinCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult UpdatePinCode([FromRoute] string accountNumber, [FromBody] UpdatePinCodeDTO pinCodeDTO)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 8)
            {
                return BadRequest("Invalid Account Number format. It must be exactly 8 characters long.");
            }

            if (!Client.UpdatePinCode(accountNumber, pinCodeDTO.NewPinCode))
            {
                return NotFound($"Client with Account Number {accountNumber} not found.");
            }

            return Ok(pinCodeDTO);
        }

        [HttpGet("account/{accountNumber}", Name = "GetClientByAccountNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClientDTO> GetClientByAccountNumber([FromRoute] string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 8)
            {
                return BadRequest("Invalid Account Number. It must be exactly 8 characters long.");
            }

            Client client = Client.FindByAccountNumber(accountNumber);

            if (client == null)
            {
                return NotFound($"Client with Account Number {accountNumber} not found.");
            }

            ClientDTO CDTO = client.CDTO;
            CDTO.PinCode = "****";
            CDTO.AccountNumber = client.AccountNumber; 

            return Ok(CDTO);
        }

        [HttpDelete("delete/{accountNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult DeleteClient([FromRoute] string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 8)
            {
                return BadRequest("Invalid Account Number format. It must be exactly 8 characters long.");
            }

            if (!Client.DeleteClientByAccountNumber(accountNumber, out string errorMessage))
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Client account closed and deleted successfully." });
        }

        [HttpPatch("deposit/{accountNumber}", Name = "DepositMoney")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClientDTO> Deposit([FromRoute] string accountNumber, [FromBody] DepositAmountDTO amountDTO)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 8)
            {
                return BadRequest("Invalid Account Number format. It must be exactly 8 characters long.");
            }

            if (!Client.Deposit(accountNumber, amountDTO.Amount, out decimal updatedBalance, out string errorMessage))
            {
                if (errorMessage.Contains("not found"))
                {
                    return NotFound(errorMessage);
                }
                return BadRequest(errorMessage);
            }

            Client client = Client.FindByAccountNumber(accountNumber);

            ClientDTO CDTO = client.CDTO;

            CDTO.PinCode = "****";
            CDTO.AccountNumber = client.AccountNumber;

            return Ok(CDTO);
        }

        [HttpPatch("withdraw/{accountNumber}", Name = "WithdrawMoney")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClientDTO> Withdraw([FromRoute] string accountNumber, [FromBody] WithdrawAmountDTO amountDTO)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 8)
            {
                return BadRequest("Invalid Account Number format. It must be exactly 8 characters long.");
            }

            if (!Client.Withdraw(accountNumber, amountDTO.Amount, out decimal updatedBalance, out string errorMessage))
            {
                if (errorMessage.Contains("not found"))
                {
                    return NotFound(errorMessage);
                }

                return BadRequest(errorMessage);
            }

            Client client = Client.FindByAccountNumber(accountNumber);

            ClientDTO CDTO = client.CDTO;

            CDTO.Balance = updatedBalance;

            CDTO.PinCode = "****";
            CDTO.AccountNumber = client.AccountNumber;

            return Ok(CDTO);
        }

        [HttpPost("transfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult ExecuteTransfer([FromBody] TransferRequestDTO transferRequest)
        {
            decimal senderNewBalance;
            string errorMessage;

            bool isSuccess = Client.TransferAmount(transferRequest, out senderNewBalance, out errorMessage);

            if (!isSuccess)
            {
                return BadRequest(errorMessage);
            }

            return Ok(senderNewBalance);
        }

        [HttpGet("logs", Name = "GetTransferLogs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<TransferLogsDTO>> GetTransferLogs()
        {
            List<TransferLogsDTO> logsList = Client.GetAllTransferLogs();

            if (logsList == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Database connection failed.");
            }

            if (logsList.Count == 0)
            {
                return NotFound("No transfer records found in the database.");
            }

            return Ok(logsList);
        }
    }
}