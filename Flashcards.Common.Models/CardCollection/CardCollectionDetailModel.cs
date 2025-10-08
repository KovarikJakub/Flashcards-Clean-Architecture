using Flashcards.Common.Enums;

namespace Flashcards.Common.Models.Card
{
    public class CardCollectionDetailModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public required DateTime AnswerStart { get; set; }
        public required DateTime AnswerEnd { get; set; }

        public IList<CardListModel> Cards { get; set; } = new List<CardListModel>();
    }
}
