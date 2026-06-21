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
        public ActionResult<IEnumerable<ClientDTO>> GetAllClients()
        {
            List<ClientDTO> clientsList = Client.GetAllClients();

            if (clientsList == null || clientsList.Count == 0)
            {
                return NotFound("No clients found in the database.");
            }

            return Ok(clientsList);
        }
    }
}