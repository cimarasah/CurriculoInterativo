using CurriculoInterativo.Api.DTOs;

namespace CurriculoInterativo.Api.Services.ContactService
{
    public interface IContactService
    {
        Task<List<ContactDto>> GetContactsAsync();
    }
}
