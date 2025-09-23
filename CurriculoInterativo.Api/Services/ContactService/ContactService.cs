using AutoMapper;
using CurriculoInterativo.Api.DTOs;
using CurriculoInterativo.Api.Repositories.ContactRepository;

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

        public async Task<List<ContactDto>> GetContactsAsync()
        {
            var contacts = await _repository.GetAllAsync();

            var contactDtos = _mapper.Map<List<ContactDto>>(contacts);

            return contactDtos;
        }
        
    }
}
