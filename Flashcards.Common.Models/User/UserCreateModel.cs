using Flashcards.Common.Enums;

namespace Flashcards.Common.Models
{
    public class UserCreateModel
    {
        public required string Name { get; set; }
        public required RoleType Role { get; set; }
        public string? PhotoUrl { get; set; } = null;
        // Identity owner id is managed by identity provider, not supplied here
    }
}
