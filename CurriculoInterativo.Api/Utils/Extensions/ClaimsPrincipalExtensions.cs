using System.Security.Claims;

namespace CurriculoInterativo.Api.Utils.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("userId")?.Value
                              ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Token inválido ou usuário não encontrado");

            return userId;
        }
    }
}
