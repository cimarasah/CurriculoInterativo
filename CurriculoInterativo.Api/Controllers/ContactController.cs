using Microsoft.AspNetCore.Mvc;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Services.ContactService;
using Microsoft.AspNetCore.Authorization;
using CurriculoInterativo.Api.Utils.Exceptions;
using System.ComponentModel.DataAnnotations;
using CurriculoInterativo.Api.Utils.Extensions;

namespace CurriculoInterativo.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(IContactService contactService, ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        /// <summary>
        /// Retorna todas as informações de contato
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ContactDto>>> GetContacts()
        {
            var contacts = await _contactService.GetContactsAsync();
            return Ok(contacts);
        }

        /// <summary>
        /// Retorna um contato específico por ID
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ContactDto>> GetContact(int id)
        {
            var contact = await _contactService.GetContactByIdAsync(id);
            if (contact == null)
                throw new NotFoundException($"Contato com ID {id} não encontrado");

            return Ok(contact);
        }

        /// <summary>
        /// Cria um novo contato
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<ContactDto>> CreateContact([FromBody] ContactDto contactDto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Dados inválidos enviados");

            var userId = User.GetUserId();

            var createdContact = await _contactService.CreateContactAsync(contactDto, userId);

            _logger.LogInformation("Contato criado com sucesso pelo usuário: {UserId}", userId);

            return CreatedAtAction(nameof(GetContact), new { id = createdContact.Id }, createdContact);
        }

        /// <summary>
        /// Atualiza um contato existente
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<ContactDto>> UpdateContact(int id, [FromBody] ContactDto contactDto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Dados inválidos enviados");

            var userId = User.GetUserId();

            var updatedContact = await _contactService.UpdateContactAsync(id, contactDto, userId);

            if (updatedContact == null)
                throw new NotFoundException($"Contato com ID {id} não encontrado");

            _logger.LogInformation("Contato {Id} atualizado com sucesso pelo usuário: {UserId}", id, userId);

            return Ok(updatedContact);
        }

        /// <summary>
        /// Exclui um contato
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var userId = User.GetUserId();

            await _contactService.DeleteContactAsync(id, userId);

            _logger.LogInformation("Contato {Id} excluído com sucesso pelo usuário: {UserId}", id, userId);

            return Ok(new { message = "Contato excluído com sucesso" });
        }
    }
}
