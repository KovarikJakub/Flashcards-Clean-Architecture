using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Common.Entities
{
    public record CardCollectionEntity : EntityBase
    {
        public required string Title { get; set; }
        public required DateTime AnswerStart { get; set; }
        public required DateTime AnswerEnd { get; set; }

        public ICollection<CardEntity> Cards { get; set; } = new List<CardEntity>();
    }
}
