using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class CardRepositoryTests
    {
        private readonly IFakeDatabase _fakeDb;
        private readonly ICardRepository _repo;
        
        public  CardRepositoryTests()
        {
            _fakeDb = new InMemoryFakeDatabase();
            _repo = _fakeDb.GetCardRepository();
        }

        public bool AreCardsEqual(CardEntity card1, CardEntity card2)
        {
            return
                card1.Id == card2.Id &&
                card1.QuestionType == card2.QuestionType &&
                card1.AnswerType == card2.AnswerType &&
                card1.QuestionContent == card2.QuestionContent &&
                card1.AnswerContent == card2.AnswerContent &&
                card1.Description == card2.Description &&
                card1.DateCreated == card2.DateCreated;
        }

        [Fact]
        public void GetById_Existing_Card()
        {
            // Act
            var requestedId = _fakeDb.CardGuids[0];
            var card = _repo.GetById(requestedId);

            // Assert
            Assert.NotNull(card);
            Assert.Equal(requestedId, card.Id);

            var expectedCard = _fakeDb.GetCardById(requestedId);
            Assert.True(AreCardsEqual(card, expectedCard));
        }

        [Fact]
        public void GetById_NonExisting_Card()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var card = _repo.GetById(nonExistingId);

            // Assert
            Assert.Null(card);
        }

        [Fact]
        public void Delete_Existing_Card()
        {
            // Arrange
            var idToRemove = _fakeDb.CardGuids[0];

            // Act
            _repo.Remove(idToRemove);

            // Assert
            Assert.Null(_fakeDb.GetCardById(idToRemove));
        }

        [Fact]
        public void Insert_New_Card()
        {
            // Arrange
            var newCard = new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is the capital of Spain?",
                AnswerContent = "Madrid",
                Description = "Geography question",
                DateCreated = new DateTime(2025, 2, 1, 10, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var newId = _repo.Insert(newCard);

            // Assert
            var inserted = _fakeDb.GetCardById(newId);
            Assert.NotNull(inserted);
            Assert.True(AreCardsEqual(inserted, newCard));
        }

        [Fact]
        public void Update_Existing_Card()
        {
            // Arrange
            var cardId = _fakeDb.CardGuids[0];
            var card = _repo.GetById(cardId);
            card.QuestionContent = "Updated Question";

            // Act
            _repo.Update(card);

            // Assert
            var updated = _fakeDb.GetCardById(cardId);
            Assert.Equal("Updated Question", updated.QuestionContent);
        }
    }
}
