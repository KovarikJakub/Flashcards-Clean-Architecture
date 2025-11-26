namespace Flashcards.Common.Models
{
    public class CardListModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string QuestionContent { get; set; }
        public string? Description { get; set; }
    }
}
