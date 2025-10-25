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

        private LessonFacade CreateLessonFacade(ILessonRepository repository)
        {
            var userMapper = new UserMapper();
            var cardCollectionMapper = new CardCollectionMapper();
            var lessonMapper = new LessonMapper(userMapper, cardCollectionMapper);
            var facade = new LessonFacade(lessonMapper, repository);
            return facade;
        }
        
        
        [Fact]
        public void Create_New_Lesson()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var newLesson = CreateNewLessonDetailModel();
            
            // Act
            facade.Create(newLesson);
            
            // Assert
            fakeRepo.Received(1).Insert(Arg.Is<LessonEntity>(lesson => lesson.Id == newLesson.Id));
        }

        [Fact]
        public void GetById_Existing_Lesson()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var newLesson = CreateNewLessonDetailModel();

            var mapper = new LessonMapper(new UserMapper(), new CardCollectionMapper());
            var lessonEntity = mapper.ToEntity(newLesson);
            
            fakeRepo.GetById(Arg.Any<Guid>()).Returns(lessonEntity);
            
            // Act
            var result = facade.GetById(newLesson.Id);

            // Assert
            fakeRepo.Received(1).GetById(lessonEntity.Id);
            Assert.NotNull(result);
            Assert.Equal(result.Id, lessonEntity.Id);
        }

        [Fact]
        public void GetById_Non_Existing_Lesson()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var nonExistingId = Guid.NewGuid();
            fakeRepo.GetById(Arg.Any<Guid>()).Returns((LessonEntity?)null);

            // Act
            var result = facade.GetById(nonExistingId);

            // Assert
            fakeRepo.Received(1).GetById(nonExistingId); 
            Assert.Null(result);
        }

        [Fact]
        public void Remove_Existing_Lesson()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var newLesson = CreateNewLessonDetailModel();
            
            // Act
            facade.Delete(newLesson.Id);
            
            // Assert
            fakeRepo.Received(1).Remove(newLesson.Id);
        }
        
        [Fact]
        public void Try_To_Remove_Non_Existing_Lesson_And_Dont_Crash()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var nonExistingId = Guid.NewGuid();
            
            // Act
            facade.Delete(nonExistingId);
            
            // Assert
            fakeRepo.Received(1).Remove(nonExistingId);
        }

        [Fact]
        public void GetAll_On_Existing_List()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var newLesson = CreateNewLessonDetailModel();

            var mapper = new LessonMapper(new UserMapper(), new CardCollectionMapper());
            var entity = mapper.ToEntity(newLesson);
            var entities = new List<LessonEntity> { entity };

            fakeRepo.GetAll().Returns(entities);
            
            // Act
            var list  = facade.GetAll();
            
            // Assert
            fakeRepo.Received(1).GetAll();
            Assert.NotEmpty(list);
            Assert.NotNull(list);
        }
        
        [Fact]
        public void GetAll_On_Non_Existing_List()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            
            fakeRepo.GetAll().Returns(new List<LessonEntity>());
            
            // Act
            var list  = facade.GetAll();
            
            // Assert
            fakeRepo.Received(1).GetAll();
            Assert.Empty(list);
            Assert.NotNull(list);
        }

        [Fact]
        public void GetByQuery_By_User()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var newLesson = CreateNewLessonDetailModel();

            var mapper = new LessonMapper(new UserMapper(), new CardCollectionMapper());
            var entity = mapper.ToEntity(newLesson);
            var entities = new List<LessonEntity> { entity };

            fakeRepo.GetAll().Returns(entities);

            var query = new LessonQueryModel{UserId = entities[0].User.Id};

            // Act
            var result = facade.GetByQuery(query);
            
            // Assert
            fakeRepo.Received(1).GetAll();
            Assert.NotNull(result);
            Assert.Single(result);
        }
        
        [Fact]
        public void GetByQuery_By_Card()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var newLesson = CreateNewLessonDetailModel();

            var mapper = new LessonMapper(new UserMapper(), new CardCollectionMapper());
            var entity = mapper.ToEntity(newLesson);
            var entities = new List<LessonEntity> { entity };

            fakeRepo.GetAll().Returns(entities);

            var query = new LessonQueryModel{CardCollectionId = entities[0].CardCollection.Id};

            // Act
            var result = facade.GetByQuery(query);
            
            // Assert
            fakeRepo.Received(1).GetAll();
            Assert.NotNull(result);
            Assert.Single(result);
        }
        
        [Fact]
        public void GetByQuery_With_No_Filter()
        {
            // Arrange
            var fakeRepo = Substitute.For<ILessonRepository>();
            var facade = CreateLessonFacade(fakeRepo);
            var newLesson = CreateNewLessonDetailModel();

            var mapper = new LessonMapper(new UserMapper(), new CardCollectionMapper());
            var entity = mapper.ToEntity(newLesson);
            var entities = new List<LessonEntity> { entity };

            fakeRepo.GetAll().Returns(entities);

            var query = new LessonQueryModel{Page = 1, PageSize = 10};

            // Act
            var result = facade.GetByQuery(query);
            
            // Assert
            fakeRepo.Received(1).GetAll();
            Assert.NotNull(result);
            Assert.Equal(entities.Count, result.Count);
        }
    }
}