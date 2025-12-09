namespace Flashcards.Common.Models
{
    public class CardCollectionListModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public required DateTime AnswerStart { get; set; }
        public required DateTime AnswerEnd { get; set; }

        public string? OwnerId { get; set; }
    }
}
