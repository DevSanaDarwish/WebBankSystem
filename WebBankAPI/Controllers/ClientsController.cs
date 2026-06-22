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
        public ActionResult<ClientDTO> AddNewClient(ClientDTO newClientDTO)
        {
            if (!Client.IsValidClient(newClientDTO, out string validationError))
            {
                return BadRequest(validationError);
            }

            Client client = new Client
                (
                new ClientDTO
                (
                    newClientDTO.ClientID,
                    newClientDTO.FirstName,
                    newClientDTO.LastName,
                    newClientDTO.Email,
                    newClientDTO.PinCode,
                    newClientDTO.Balance.Value,
                    newClientDTO.AccountNumber
                )
            );

            if (!client.Save())
            {
                return StatusCode(500, "An internal server error occurred while adding the new client.");
            }

            newClientDTO.ClientID = client.ClientID;

            return CreatedAtRoute("GetClientById", new { id = newClientDTO.ClientID }, newClientDTO);
        }


        [HttpGet("{id}", Name = "GetClientById")]
        public ActionResult<ClientDTO> GetClientById(int id)
        {

        }
    }
}