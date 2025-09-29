using Flashcards.Api.DAL.Common.Entities;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class CardRepositoryTests
    {
        private readonly IFakeDatabase fakeDb;
        public CardRepositoryTests()
        {
            fakeDb = new InMemoryFakeDatabase();
        }

        private bool CardsEqual(CardEntity left, CardEntity right)
        {
            return
                left.Id == right.Id &&
                left.AnswerType == right.AnswerType &&
                left.AnswerContent == right.AnswerContent &&
                left.QuestionType == right.QuestionType &&
                left.QuestionContent == right.QuestionContent &&
                left.DateCreated == right.DateCreated &&
                left.Description == right.Description;
        }

        [Fact]
        public void GetById_Returns_Requested_Card()
        {
            // Arrange
            var repository = fakeDb.GetCardRepository();

            // Act
            var requestedId = fakeDb.CardGuids[0];
            var card = repository.GetById(requestedId);

            // Assert
            Assert.NotNull(card);
            Assert.Equal(card.Id, requestedId);
            
            var expectedCard = fakeDb.GetCardById(requestedId);
            Assert.True(CardsEqual(card, expectedCard));
        }
    }
}
