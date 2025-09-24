using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CurriculoInterativo.Api.Utils.Attributes
{
    public class OwnerOnlyAttribute : AuthorizeAttribute
    {
        public OwnerOnlyAttribute()
        {
            Roles = "Owner";
        }
    }
}
