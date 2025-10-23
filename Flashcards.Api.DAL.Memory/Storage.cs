
using System.ComponentModel;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Memory
{
    public class Storage
    {
        public IList<CardEntity> Cards { get; } = new List<CardEntity>();
        public IList<UserEntity> Users { get; } = new List<UserEntity>();
        public IList<CardCollectionEntity> Collections { get; } = new List<CardCollectionEntity>();

        public IList<LessonEntity> Lessons { get; } = new List<LessonEntity>();

        public Storage(bool createWithDummyData = true) 
        {
            if (createWithDummyData)
            {
                SeedCards();
                SeedUsers();
                SeedCollections();
                SeedLessons();
            }
        }

        private void SeedCards()
        {
            Cards.Add(new CardEntity
            {
                Id = new("7f251cd6-3ac4-49be-b3e7-d1f9f7cfdd3a"),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is the capital of France?",
                AnswerContent = "Paris",
                DateCreated = DateTime.UtcNow,
                Description = "A geography question."
            });

            Cards.Add(new CardEntity
            {
                Id = new("23b3902d-7d4f-4213-9cf0-112348f56238"),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is 2 + 2?",
                AnswerContent = "4",
                DateCreated = DateTime.UtcNow,
                Description = "A simple math question."
            });

            Cards.Add(new CardEntity
            {
                Id = new("df935095-8709-4040-a2bb-b6f97cb416dc"),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "Who wrote 'Hamlet'?",
                AnswerContent = "William Shakespeare",
                DateCreated = DateTime.UtcNow,
                Description = "A literature question."
            });
        }

        private void SeedUsers()
        {
            Users.Add(new UserEntity
            {
                Id = new Guid("7e664a56-e7b8-4301-84ad-ec08446465f2"),
                Name = "Filip", 
                PhotographyUrl = String.Empty,
                Role = RoleType.Administrator
            });
            
            Users.Add(new UserEntity
            {
                Id = new Guid("fb4049db-148d-44c2-9be4-5abeffe971a2"),
                Name = "Prokop", 
                PhotographyUrl = String.Empty,
                Role = RoleType.User
            });
            
            Users.Add(new UserEntity
            {
                Id = new Guid("241e43ef-e4ac-4dd6-8176-fb0ba2de4669"),
                Name = "Petr", 
                PhotographyUrl = String.Empty,
                Role = RoleType.User
            });
        }

        private void SeedCollections()
        {
            Collections.Add(new CardCollectionEntity
            {
                Id = new("b72b54c4-9f35-4e3f-96a8-3fdcbbbc09ee"),
                Title = "placeholder",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow
            });

           
        }

        private void SeedLessons()
        {
            Lessons.Add(new LessonEntity
            {
                User = Users[0],
                CardCollection = Collections[0],
                Id = new ("536adfc4-8284-404d-a00e-b4a007413a44"),
                Start = DateTime.UtcNow,
                Seen = 1,
                Correct = 1,
                Wrong = 0
            });
        }
    }
}
