using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public record CreateUserWithIdentityRequest(
        string UserName,
        string Password,
        string Email,
        string Name,
        string? PhotographyUrl,
        RoleType Role);
}