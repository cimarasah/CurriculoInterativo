using CurriculoInterativo.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurriculoInterativo.Api.Repositories.ContactRepository
{
    public class ContactRepository(ResumeDbContext context) : BaseRepository<Contact>(context), IContactRepository
    {
    }
}
