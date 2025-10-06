using Flashcards.Common.Enums;

namespace Flashcards.Common.Models.User
{
    public class UserListModel : IWithGuid
    {
        public Guid Id { get; init; }

        public required string Name { get; set; }
        
        public required RoleType Role { get; set; }
    }
}

