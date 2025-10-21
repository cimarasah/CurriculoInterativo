using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Entities;
using CurriculoInterativo.Api.Repositories.ContactRepository;
using CurriculoInterativo.Api.Utils.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace CurriculoInterativo.Api.Services.ContactService
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _repository;
        private readonly IMapper _mapper;

        public ContactService(IContactRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ContactRequest>> GetContactsAsync()
        {
            var contacts = await _repository.GetAllAsync();

            var contactDtos = _mapper.Map<List<ContactRequest>>(contacts);

            return contactDtos;
        }
        public async Task<ContactRequest?> GetContactByIdAsync(int id)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null)
                throw new NotFoundException($"Contato com ID {id} não encontrado");

            return _mapper.Map<ContactRequest>(contact);
        }

        public async Task<ContactRequest> CreateContactAsync(ContactRequest contactDto, int userId)
        {
            if (contactDto == null)
                throw new ValidationException("Dados do contato não podem ser nulos");

            var contact = _mapper.Map<Contact>(contactDto);

            await _repository.AddAsync(contact);

            return _mapper.Map<ContactRequest>(contact);
        }

        public async Task<ContactRequest?> UpdateContactAsync(int id, ContactRequest contactDto, int userId)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null)
                throw new NotFoundException($"Contato com ID {id} não encontrado");

            _mapper.Map(contactDto, contact); 

            await _repository.UpdateAsync(contact);

            return _mapper.Map<ContactRequest>(contact);
        }

        public async Task<bool> DeleteContactAsync(int id, int userId)
        {
            var contact = await _repository.GetByIdAsync(id);
            if (contact == null)
                throw new NotFoundException($"Contato com ID {id} não encontrado");

            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
