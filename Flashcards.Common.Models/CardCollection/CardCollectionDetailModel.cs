using Flashcards.Common.Enums;
using Flashcards.Common.Models.Card;

namespace Flashcards.Common.Models.CardCollection
{
    public class CardCollectionDetailModel : IWithGuid
    {
        public required Guid Id { get; init; }
        public required string Title { get; set; }
        public required DateTime AnswerStart { get; set; }
        public required DateTime AnswerEnd { get; set; }

        public IList<CardDetailModel> Cards { get; set; } = new List<CardDetailModel>();
    }
}
