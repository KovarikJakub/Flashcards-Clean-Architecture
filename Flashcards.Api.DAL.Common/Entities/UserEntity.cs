using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Common.Entities
{
    public record UserEntity : EntityBase
    {
        public required string Name { get; set; }

        public required string? PhotographyUrl { get; set; }
        
        public required RoleType Role { get; set; }
            
    }
}

