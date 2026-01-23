using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;
using Flashcards.Common.Enums;
using NSubstitute;

namespace Flashcards.Api.BL.UnitTests
{
    public class CardFacadeTests
    {
        private readonly ICardRepository _fakeRepo;
        private readonly CardFacade _facade;
        private readonly CardMapper _mapper;

        public CardFacadeTests()
        {
            _mapper = new CardMapper();
            _fakeRepo = Substitute.For<ICardRepository>();
            _facade = new CardFacade(_fakeRepo, _mapper);
        }
        private CardDetailModel CreateCardDetailModel()
        {
            var newCard = new CardDetailModel
            {
                Id = new("23b3902d-7d4f-4213-9cf0-112348f56238"),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is 2 + 2?",
                AnswerContent = "4",
                DateCreated = DateTime.UtcNow,
                Description = "A simple math question."
            };

            return newCard;
        }

        [Fact]
        public void Delete_Calls_Method_On_Repository_Correctly()
        {
            // Arrange
            var cardGuid = Guid.NewGuid();

            // Act
            _facade.Delete(cardGuid);

            // Assert
            _fakeRepo.Received().Remove(cardGuid);
        }

        [Fact]
        public void Create_New_Card()
        {
            // Arrange
            var newCard = CreateCardDetailModel();

            // Act
            var result = _facade.Create(newCard);

            // Assert
            _fakeRepo.Received(1).Insert(Arg.Is<CardEntity>(card => card.Id == newCard.Id));
            Assert.Equal(result, newCard.Id);
        }

        [Fact]
        public void Create_New_Card_With_CreateOrUpdate()
        {
            // Arrange
            var newCard = CreateCardDetailModel();

            // Act
            var result = _facade.CreateOrUpdate(newCard);

            // Assert
            _fakeRepo.Received(1).Insert(Arg.Is<CardEntity>(card => card.Id == newCard.Id));
            Assert.Equal(result, newCard.Id);
        }

        [Fact]
        public void Update_Existing_Card_With_CreateOrUpdate()
        {
            // Arrange
            var existingCard = CreateCardDetailModel();
            _fakeRepo.Exists(existingCard.Id).Returns(true);
            existingCard.Description = "updated value";
            var expectedEntity = _mapper.ToEntity(existingCard);
            _fakeRepo.Update(Arg.Any<CardEntity>()).Returns(existingCard.Id);

            // Act
            var result = _facade.CreateOrUpdate(existingCard);
            
            // Assert
            _fakeRepo.DidNotReceive().Insert(Arg.Any<CardEntity>());
            
            _fakeRepo.Received(1).Update(Arg.Is<CardEntity>(
                card => card.Id == existingCard.Id && card.Description == "updated value"));
            
            Assert.Equal(existingCard.Id, result);
        }

        [Fact]
        public void GetById_On_Existing_Card()
        {
            // Arrange
            var newCard = CreateCardDetailModel();
            _fakeRepo.GetById(Arg.Any<Guid>()).Returns(_mapper.ToEntity(newCard));
            
            // Act
            var result = _facade.GetById(newCard.Id);
            
            // Assert
            _fakeRepo.Received(1).GetById(newCard.Id);
            Assert.NotNull(result);
            Assert.Equal(newCard.Id, result.Id);
        }
        
        [Fact]
        public void GetById_On_Non_Existing_Card()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            _fakeRepo.GetById(Arg.Any<Guid>()).Returns((CardEntity?)null);
            
            // Act
            var result = _facade.GetById(nonExistingId);
            
            // Assert
            _fakeRepo.Received(1).GetById(nonExistingId);
            Assert.Null(result);
        }

        [Fact]
        public void GetAll_On_Empty_Repo()
        {
            // Arrange
            _fakeRepo.GetAll().Returns(new List<CardEntity>());

            // Act
            var result = _facade.GetAll();

            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.Empty(result);
        }
        
        [Fact]
        public void GetAll_On_Non_Empty_Repo()
        {
            // Arrange
            var newCard = CreateCardDetailModel();
            var entities = new List<CardEntity>{ _mapper.ToEntity(newCard) };
            _fakeRepo.GetAll().Returns(entities);

            // Act
            var result = _facade.GetAll();

            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.NotEmpty(result);
        }
        
        [Fact]
        public void Update_Existing_Card_With()
        {
            // Arrange
            var existingCard = CreateCardDetailModel();
            _fakeRepo.Exists(existingCard.Id).Returns(true);
            existingCard.Description = "updated value";
            var expectedEntity = _mapper.ToEntity(existingCard);
            _fakeRepo.Update(Arg.Any<CardEntity>()).Returns(existingCard.Id);

            // Act
            var result = _facade.Update(existingCard);
            
            // Assert
            _fakeRepo.DidNotReceive().Insert(Arg.Any<CardEntity>());
            _fakeRepo.Received(1).Update(Arg.Is<CardEntity>(
                card => card.Id == existingCard.Id && card.Description == "updated value"));
            
            Assert.Equal(existingCard.Id, result);
        }
        
        [Fact]
        public void Update_On_Non_Existing_Card_With()
        {
            // Arrange
            var card = CreateCardDetailModel();
            _fakeRepo.Exists(Arg.Any<Guid>()).Returns(false);

            // Act
            var result = _facade.Update(card);
            
            // Assert
            _fakeRepo.DidNotReceive().Insert(Arg.Any<CardEntity>());
            _fakeRepo.Received(1).Update(_mapper.ToEntity(card));
            Assert.Null(result);
        }

        [Fact]
        public void GetByQuery_Filters_By_Question_Text()
        {
            // Arrange
            var cards = new List<CardEntity>()
            {
                new()
                {
                    Id = new("7f251cd6-3ac4-49be-b3e7-d1f9f7cfdd3a"),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "What is the capital of France?",
                    AnswerContent = "Paris",
                    Created = DateTime.UtcNow,
                    Description = "A geography question."
                },
                new()
                {
                    Id = new("23b3902d-7d4f-4213-9cf0-112348f56238"),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "What is 2 + 2?",
                    AnswerContent = "4",
                    Created = DateTime.UtcNow,
                    Description = "A simple math question."
                },
                new()
                {
                    Id = new("df935095-8709-4040-a2bb-b6f97cb416dc"),
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Who wrote 'Hamlet'?",
                    AnswerContent = "William Shakespeare",
                    Created = DateTime.UtcNow,
                    Description = "A literature question."
                }
            };
            _fakeRepo.GetAll().Returns(cards);

            var query = new CardQueryModel() { SearchString = "What" };
            var filtered = _facade.GetByQuery(query);

            Assert.NotNull(filtered);
            Assert.Equal(2, filtered.Count());
        }

        [Fact]
        public void GetByQuery_Filters_By_QuestionTextAndDescription()
        {
            var cards = new List<CardEntity>()
            {
                new()
                {
                    Id = Guid.Empty,
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Question1",
                    AnswerContent = "Answer1",
                    Created = DateTime.UtcNow,
                    Description = "Desc1."
                },
                new()
                {
                    Id = Guid.Empty,
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Question2FINDME",
                    AnswerContent = "Answer2",
                    Created = DateTime.UtcNow,
                    Description = "Desc3"
                },
                new()
                {
                    Id = Guid.Empty,
                    QuestionType = ContentType.Text,
                    AnswerType = ContentType.Text,
                    QuestionContent = "Question3",
                    AnswerContent = "Answer3",
                    Created = DateTime.UtcNow,
                    Description = "Desc3FINDME_"
                }
            };

            _fakeRepo.GetAll().Returns(cards);

            var query = new CardQueryModel() { SearchString = "FINDME" };
            var filtered = _facade.GetByQuery(query);

            Assert.NotNull(filtered);
            Assert.Equal(2, filtered.Count());
            // Assert Questions 2 and 3 are in the result:
            Assert.Contains(filtered, card => card.QuestionContent == "Question2FINDME");
            Assert.Contains(filtered, card => card.QuestionContent == "Question3");
        }
    }
}
