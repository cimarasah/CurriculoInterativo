using CurriculoInterativo.Api.DTOs;

namespace CurriculoInterativo.Api.Services.ContactService
{
    public interface IContactService
    {
        Task<List<ContactDto>> GetContactsAsync();

        Task<ContactDto?> GetContactByIdAsync(int id);

        Task<ContactDto> CreateContactAsync(ContactDto contactDto, int userId);

        Task<ContactDto?> UpdateContactAsync(int id, ContactDto contactDto, int userId);

        Task<bool> DeleteContactAsync(int id, int userId);
    }
}
