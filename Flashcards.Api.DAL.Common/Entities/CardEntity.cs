using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Common.Entities
{
    public record CardEntity : EntityBase
    {
        public required ContentType QuestionType { get; set; }
        public required ContentType AnswerType { get; set; }
        public required string QuestionContent { get; set; }
        public required string AnswerContent { get; set; }
        public required DateTime Created { get; set; }
        public string? Description { get; set; }
    }
}
