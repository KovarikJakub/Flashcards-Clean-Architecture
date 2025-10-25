using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class CardCollectionRepositoryTests
    {
        private readonly IFakeDatabase _fakeDb;
        private readonly ICardCollectionRepository _repo;

        public CardCollectionRepositoryTests()
        {
            _fakeDb = new InMemoryFakeDatabase();
            _repo = _fakeDb.GetCardCollectionRepository();
        }

        private bool AreCollectionsEqual(CardCollectionEntity c1, CardCollectionEntity c2)
        {
            return c1.Id == c2.Id &&
                   c1.Title == c2.Title &&
                   c1.AnswerStart == c2.AnswerStart &&
                   c1.Cards == c2.Cards &&
                   c1.AnswerEnd == c2.AnswerEnd;
        }

        [Fact]
        public void GetById_Existing_Collection()
        {
            // Act
            var requestedId = _fakeDb.CardCollectionGuids[0];
            var collection = _repo.GetById(requestedId);

            // Assert
            Assert.NotNull(collection);
            Assert.Equal(collection.Id, requestedId);

            var expectedCollection = _fakeDb.GetCardCollectionById(requestedId);
            Assert.NotNull(expectedCollection);
            Assert.True(AreCollectionsEqual(collection, expectedCollection));
        }

        [Fact]
        public void GetById_Non_Existing_Collection()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();

            // Act
            var collection = _repo.GetById(nonExistingId);

            // Assert
            Assert.Null(collection);
        }

        [Fact]
        public void Delete_Existing_Collection()
        {
            // Arrange
            var idToRemove = _fakeDb.CardCollectionGuids[0];

            // Act
            _repo.Remove(idToRemove);

            // Assert
            Assert.Null(_fakeDb.GetCardCollectionById(idToRemove));
        }

        [Fact]
        public void Insert_New_Collection()
        {
            // Arrange
            var newCollection = new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "Math Formulas",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddHours(2),
                Cards = new List<CardEntity> {  }
            };

            // Act
            var newId = _repo.Insert(newCollection);

            // Assert
            Assert.NotNull(_fakeDb.GetCardCollectionById(newId));
        }

        [Fact]
        public void Update_Existing_Collection()
        {
            // Arrange
            var collectionId = _fakeDb.CardCollectionGuids[0];
            var collection = _repo.GetById(collectionId);
            collection.Title = "Updated Collection Title";

            // Act
            _repo.Update(collection);

            // Assert
            var expectedCollection = _fakeDb.GetCardCollectionById(collectionId);
            Assert.NotNull(expectedCollection);
            Assert.Equal(collection.Title, expectedCollection.Title);
        }
    }
}
