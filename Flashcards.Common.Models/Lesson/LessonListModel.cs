using Flashcards.Common.Models.CardCollection;

namespace Flashcards.Common.Models.Lesson
{
    public class LessonListModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string CardCollectionName { get; set; }
        public required int CardCount { get; set; }
        public required DateTime Start { get; set; }
        public required uint Seen { get; set; }
        public required uint Wrong { get; set; }
        public required uint Correct { get; set; }
    }
}
