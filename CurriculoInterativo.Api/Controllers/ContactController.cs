using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Services.ContactService;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        /// <summary>
        /// Retorna as informações de contato
        /// </summary>
        /// <returns>Informações de contato</returns>
        [HttpGet]
        public async Task<ActionResult<ContactDto>> GetContato()
        {
            try
            {
                var contact = await _contactService.GetContactsAsync();
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}

