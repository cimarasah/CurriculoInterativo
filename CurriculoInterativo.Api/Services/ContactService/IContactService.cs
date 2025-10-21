using CurriculoInterativo.Api.DTOs;

namespace CurriculoInterativo.Api.Services.ContactService
{
    public interface IContactService
    {
        Task<List<ContactRequest>> GetContactsAsync();

        Task<ContactRequest?> GetContactByIdAsync(int id);

        Task<ContactRequest> CreateContactAsync(ContactRequest contactDto, int userId);

        Task<ContactRequest?> UpdateContactAsync(int id, ContactRequest contactDto, int userId);

        Task<bool> DeleteContactAsync(int id, int userId);
    }
}
