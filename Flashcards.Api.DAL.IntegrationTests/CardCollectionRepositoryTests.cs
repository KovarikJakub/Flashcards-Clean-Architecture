using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class CardCollectionRepositoryTests
    {
        private readonly IFakeDatabase fakeDb;

        public CardCollectionRepositoryTests()
        {
            fakeDb = new InMemoryFakeDatabase();
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
            // Arrange
            var repo = fakeDb.GetCardCollectionRepository();

            // Act
            var requestedId = fakeDb.CardCollectionGuids[0];
            var collection = repo.GetById(requestedId);

            // Assert
            Assert.NotNull(collection);
            Assert.Equal(collection.Id, requestedId);

            var expectedCollection = fakeDb.GetCardCollectionById(requestedId);
            Assert.True(AreCollectionsEqual(collection, expectedCollection));
        }

        [Fact]
        public void GetById_Non_Existing_Collection()
        {
            // Arrange
            var repo = fakeDb.GetCardCollectionRepository();
            var nonExistingId = Guid.NewGuid();

            // Act
            var collection = repo.GetById(nonExistingId);

            // Assert
            Assert.Null(collection);
        }

        [Fact]
        public void Delete_Existing_Collection()
        {
            // Arrange
            var repo = fakeDb.GetCardCollectionRepository();
            var idToRemove = fakeDb.CardCollectionGuids[0];

            // Act
            repo.Remove(idToRemove);

            // Assert
            Assert.Null(fakeDb.GetCardCollectionById(idToRemove));
        }

        [Fact]
        public void Insert_New_Collection()
        {
            // Arrange
            var repo = fakeDb.GetCardCollectionRepository();
            var newCollection = new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "Math Formulas",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddHours(2),
                Cards = new List<CardEntity> {  }
            };

            // Act
            var newId = repo.Insert(newCollection);

            // Assert
            Assert.NotNull(fakeDb.GetCardCollectionById(newId));
        }

        [Fact]
        public void Update_Existing_Collection()
        {
            // Arrange
            var repo = fakeDb.GetCardCollectionRepository();

            var collectionId = fakeDb.CardCollectionGuids[0];
            var collection = repo.GetById(collectionId);
            collection.Title = "Updated Collection Title";

            // Act
            repo.Update(collection);

            // Assert
            var expectedCollection = fakeDb.GetCardCollectionById(collectionId);
            Assert.Equal(collection.Title, expectedCollection.Title);
        }
    }
}
