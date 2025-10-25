using Flashcards.Api.BL.Facades;
using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models.Card;
using Flashcards.Common.Models.CardCollection;
using Flashcards.Common.Models.Lesson;
using Flashcards.Common.Models.User;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Flashcards.Api.BL.UnitTests
{
    public class LessonFacadeTests
    {
        private readonly ILessonRepository _fakeRepo;
        private readonly LessonMapper _mapper;
        private readonly LessonFacade _facade;

        public LessonFacadeTests()
        {
            _fakeRepo = Substitute.For<ILessonRepository>();
            _mapper = new LessonMapper(new UserMapper(), new CardCollectionMapper());
            _facade = new LessonFacade(_mapper, _fakeRepo);
        }
        
        private LessonDetailModel CreateNewLessonDetailModel()
        {
            var newUser = new UserDetailModel
            {
                Id = Guid.NewGuid(),
                Name = "Albert",
                PhotographyUrl = null,
                Role = RoleType.User
            };

            var newCard1 = new CardDetailModel
            {
                Id = new("7f251cd6-3ac4-49be-b3e7-d1f9f7cfdd3a"),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is the capital of France?",
                AnswerContent = "Paris",
                DateCreated = DateTime.UtcNow,
                Description = "A geography question."
            };

            var newCard2 = new CardDetailModel
            {
                Id = new("23b3902d-7d4f-4213-9cf0-112348f56238"),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is 2 + 2?",
                AnswerContent = "4",
                DateCreated = DateTime.UtcNow,
                Description = "A simple math question."
            };

            IList<CardDetailModel> list = new List<CardDetailModel>();
            list.Add(newCard1);
            list.Add(newCard2);
                
            var newCardCollection = new CardCollectionDetailModel
            {
                Id = Guid.NewGuid(),
                AnswerEnd = new DateTime(2025, 11, 10),
                AnswerStart = new DateTime(2025, 11, 9),
                Cards = list,
                Title = "Test Prep cards"   
            };

            var newLesson = new LessonDetailModel
            {
                CardCollection = newCardCollection,
                Correct = 9,
                Id = Guid.NewGuid(),
                Seen = 10,
                Start = DateTime.Today,
                User = newUser, Wrong = 2
            };

            return newLesson;
        }
        
        [Fact]
        public void Create_New_Lesson()
        {
            // Arrange
            var newLesson = CreateNewLessonDetailModel();
            
            // Act
            _facade.Create(newLesson);
            
            // Assert
            _fakeRepo.Received(1).Insert(Arg.Is<LessonEntity>(lesson => lesson.Id == newLesson.Id));
        }

        [Fact]
        public void GetById_Existing_Lesson()
        {
            // Arrange
            var newLesson = CreateNewLessonDetailModel();
            var lessonEntity = _mapper.ToEntity(newLesson);
            _fakeRepo.GetById(Arg.Any<Guid>()).Returns(lessonEntity);
            
            // Act
            var result = _facade.GetById(newLesson.Id);

            // Assert
            _fakeRepo.Received(1).GetById(lessonEntity.Id);
            Assert.NotNull(result);
            Assert.Equal(result.Id, lessonEntity.Id);
        }

        [Fact]
        public void GetById_Non_Existing_Lesson()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            _fakeRepo.GetById(Arg.Any<Guid>()).Returns((LessonEntity?)null);

            // Act
            var result = _facade.GetById(nonExistingId);

            // Assert
            _fakeRepo.Received(1).GetById(nonExistingId); 
            Assert.Null(result);
        }

        [Fact]
        public void Remove_Existing_Lesson()
        {
            // Arrange
            var newLesson = CreateNewLessonDetailModel();
            
            // Act
            _facade.Delete(newLesson.Id);
            
            // Assert
            _fakeRepo.Received(1).Remove(newLesson.Id);
        }
        
        [Fact]
        public void Try_To_Remove_Non_Existing_Lesson_And_Dont_Crash()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            
            // Act
            _facade.Delete(nonExistingId);
            
            // Assert
            _fakeRepo.Received(1).Remove(nonExistingId);
        }

        [Fact]
        public void GetAll_On_Existing_List()
        {
            // Arrange
            var newLesson = CreateNewLessonDetailModel();
            var entities = new List<LessonEntity> { _mapper.ToEntity(newLesson) };
            _fakeRepo.GetAll().Returns(entities);
            
            // Act
            var list  = _facade.GetAll();
            
            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.NotEmpty(list);
            Assert.NotNull(list);
        }
        
        [Fact]
        public void GetAll_On_Non_Existing_List()
        {
            // Arrange
            _fakeRepo.GetAll().Returns(new List<LessonEntity>());
            
            // Act
            var list  = _facade.GetAll();
            
            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.Empty(list);
            Assert.NotNull(list);
        }

        [Fact]
        public void GetByQuery_By_User()
        {
            // Arrange
            var newLesson = CreateNewLessonDetailModel();
            var entities = new List<LessonEntity> { _mapper.ToEntity(newLesson) };
            _fakeRepo.GetAll().Returns(entities);
            var query = new LessonQueryModel{UserId = entities[0].User.Id};

            // Act
            var result = _facade.GetByQuery(query);
            
            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.NotNull(result);
            Assert.Single(result);
        }
        
        [Fact]
        public void GetByQuery_By_Card()
        {
            // Arrange
            var newLesson = CreateNewLessonDetailModel();
            var entities = new List<LessonEntity> { _mapper.ToEntity(newLesson) };
            _fakeRepo.GetAll().Returns(entities);
            var query = new LessonQueryModel{CardCollectionId = entities[0].CardCollection.Id};

            // Act
            var result = _facade.GetByQuery(query);
            
            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.NotNull(result);
            Assert.Single(result);
        }
        
        [Fact]
        public void GetByQuery_With_No_Filter()
        {
            // Arrange
            var newLesson = CreateNewLessonDetailModel();
            var entities = new List<LessonEntity> { _mapper.ToEntity(newLesson) };
            _fakeRepo.GetAll().Returns(entities);
            var query = new LessonQueryModel{Page = 1, PageSize = 10};

            // Act
            var result = _facade.GetByQuery(query);
            
            // Assert
            _fakeRepo.Received(1).GetAll();
            Assert.NotNull(result);
            Assert.Equal(entities.Count, result.Count);
        }
    }
}