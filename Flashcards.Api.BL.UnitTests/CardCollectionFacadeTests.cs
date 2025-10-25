using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models.Card;
using Flashcards.Common.Models.CardCollection;
using NSubstitute;

namespace Flashcards.Api.BL.UnitTests
{
    public class CardCollectionFacadeTests
    {
        [Fact]
        public void Delete_Calls_Correct_Method_On_Repository()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);
            var collectionId = Guid.NewGuid();

            // Act
            facade.Delete(collectionId);

            // Assert
            fakeRepo.Received(1).Remove(collectionId);
        }

        [Fact]
        public void Create_New_Collection()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);
            var newCollection = new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                Title = "Geography Facts",
                AnswerStart = DateTime.UtcNow.AddMinutes(-5),
                AnswerEnd = DateTime.UtcNow.AddMinutes(5),
                Cards = new List<CardDetailModel>()
            };

            // Act
            var id = facade.Create(newCollection);

            // Assert
            fakeRepo.Received(1).Insert(Arg.Is<CardCollectionEntity>(c =>
                c.Id == newCollection.Id &&
                c.Title == newCollection.Title &&
                c.AnswerStart == newCollection.AnswerStart &&
                c.AnswerEnd == newCollection.AnswerEnd));

            Assert.Equal(id, newCollection.Id);
        }

        [Fact]
        public void CreateOrUpdate_On_New_Collection()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);
            var newCollection = new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                Title = "History Deck",
                AnswerStart = DateTime.UtcNow.AddHours(-1),
                AnswerEnd = DateTime.UtcNow.AddHours(1),
                Cards = new List<CardDetailModel>()
            };

            // Act
            var id = facade.CreateOrUpdate(newCollection);

            // Assert
            fakeRepo.Received(1).Insert(Arg.Is<CardCollectionEntity>(c =>
                c.Id == newCollection.Id &&
                c.Title == newCollection.Title));

            Assert.Equal(id, newCollection.Id);
        }

        [Fact]
        public void CreateOrUpdate_On_Existing_Collection()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            fakeRepo.Exists(Arg.Any<Guid>()).Returns(true);
            fakeRepo.Update(Arg.Any<CardCollectionEntity>()).Returns(Guid.NewGuid());

            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);

            var existing = new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                Title = "Science Deck",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddMinutes(15),
                Cards = new List<CardDetailModel>()
            };

            existing.Title = "Updated Science Deck";

            // Act
            facade.CreateOrUpdate(existing);

            // Assert
            fakeRepo.Received(1).Update(Arg.Is<CardCollectionEntity>(c =>
                c.Id == existing.Id &&
                c.Title == "Updated Science Deck"));
        }

        [Fact]
        public void GetById_NonExisting_Returns_Null()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            fakeRepo.GetById(Arg.Any<Guid>()).Returns((CardCollectionEntity?)null);
            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);
            var id = Guid.NewGuid();

            // Act
            var result = facade.GetById(id);

            // Assert
            fakeRepo.Received(1).GetById(id);
            Assert.Null(result);
        }

        [Fact]
        public void GetById_Existing_Returns_Model()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            var existing = new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "Math Deck",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddMinutes(30)
            };
            fakeRepo.GetById(existing.Id).Returns(existing);

            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);

            // Act
            var result = facade.GetById(existing.Id);

            // Assert
            fakeRepo.Received(1).GetById(existing.Id);
            Assert.NotNull(result);
            Assert.Equal(existing.Id, result!.Id);
            Assert.Equal(existing.Title, result.Title);
        }

        [Fact]
        public void GetAll_On_Existing_Collections()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);

            var col1 = new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "History Deck",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddMinutes(5)
            };
            var col2 = new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "Geography Deck",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddMinutes(5)
            };

            var list = new List<CardCollectionEntity> { col1, col2 };
            fakeRepo.GetAll().Returns(list);

            // Act
            var result = facade.GetAll();

            // Assert
            fakeRepo.Received(1).GetAll();
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Id == col1.Id && c.Title == "History Deck");
            Assert.Contains(result, c => c.Id == col2.Id && c.Title == "Geography Deck");
        }

        [Fact]
        public void GetAll_On_Empty_Repo()
        {
            // Arrange
            var fakeRepo = Substitute.For<ICardCollectionRepository>();
            fakeRepo.GetAll().Returns(new List<CardCollectionEntity>());

            var mapper = new CardCollectionMapper();
            var facade = new CardCollectionFacade(fakeRepo, mapper);

            // Act
            var result = facade.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            fakeRepo.Received(1).GetAll();
        }
    }
}
