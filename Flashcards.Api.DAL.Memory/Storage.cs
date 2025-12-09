using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Interfaces;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Memory
{
    public class Storage : IStorage
    {
        public IList<CardEntity> Cards { get; } = new List<CardEntity>();
        public IList<UserEntity> Users { get; } = new List<UserEntity>();
        public IList<CardCollectionEntity> Collections { get; } = new List<CardCollectionEntity>();
        public IList<LessonEntity> Lessons { get; } = new List<LessonEntity>();

        public Storage(bool createWithDummyData = true)
        {
            if (createWithDummyData)
            {
                Seed();
            }
        }

        public void Seed()
        {
            Clear();          
            SeedUsers();
            SeedCards();
            SeedCollections();
            SeedLessons();
        }

        public void Clear()
        {
            Cards.Clear();
            Users.Clear();
            Collections.Clear();
            Lessons.Clear();
        }

        private void SeedCards()
        {
            // --- General Knowledge ---
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

            // --- Programming (C#) ---
            Cards.Add(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What keyword is used to inherit from a class in C#?",
                AnswerContent = ": (colon)",
                DateCreated = DateTime.UtcNow,
                Description = "C# Syntax"
            });

            Cards.Add(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is the default access modifier for class members in C#?",
                AnswerContent = "Private",
                DateCreated = DateTime.UtcNow,
                Description = "C# Basics"
            });

            // --- Biology ---
            Cards.Add(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is the powerhouse of the cell?",
                AnswerContent = "Mitochondria",
                DateCreated = DateTime.UtcNow,
                Description = "Cell biology"
            });

            Cards.Add(new CardEntity
            {
                Id = Guid.NewGuid(),
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "How many bones are in the adult human body?",
                AnswerContent = "206",
                DateCreated = DateTime.UtcNow,
                Description = "Anatomy"
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
                Name = "JakubUser",
                PhotographyUrl = String.Empty,
                Role = RoleType.User,
                OwnerId = "Jakub"
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
            // Collection 1: General Mix (Admin Owned)
            var mixCards = Cards.Where(c =>
                c.Id == new Guid("7f251cd6-3ac4-49be-b3e7-d1f9f7cfdd3a") ||
                c.Id == new Guid("23b3902d-7d4f-4213-9cf0-112348f56238") ||
                c.Id == new Guid("df935095-8709-4040-a2bb-b6f97cb416dc")
            ).ToList();

            Collections.Add(new CardCollectionEntity
            {
                Id = new("b72b54c4-9f35-4e3f-96a8-3fdcbbbc09ee"),
                Title = "General Knowledge Mix",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddMinutes(10),
                OwnerId = Users[0].Id.ToString(), 
                Cards = mixCards
            });

            // Collection 2: C# Programming (User Owned)
            var csharpCards = Cards.Where(c => c.Description != null && c.Description.Contains("C#")).ToList();

            Collections.Add(new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "C# Interview Prep",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddDays(1),
                OwnerId = "Jakub",
                Cards = csharpCards
            });

            // Collection 3: Biology 101 (User Owned)
            var bioCards = Cards.Where(c => c.Description == "Cell biology" || c.Description == "Anatomy").ToList();

            Collections.Add(new CardCollectionEntity
            {
                Id = Guid.NewGuid(),
                Title = "Biology 101",
                AnswerStart = DateTime.UtcNow,
                AnswerEnd = DateTime.UtcNow.AddHours(2),
                OwnerId = Users[2].Id.ToString(),
                Cards = bioCards
            });
        }

        private void SeedLessons()
        {
            if (Users.Any() && Collections.Any())
            {
                
                Lessons.Add(new LessonEntity
                {
                    User = Users[0],
                    CardCollection = Collections[0],
                    Id = new("536adfc4-8284-404d-a00e-b4a007413a44"),
                    Start = DateTime.UtcNow,
                    Seen = 3,
                    Correct = 2,
                    Wrong = 1
                });

                
                if (Collections.Count > 1)
                {
                    Lessons.Add(new LessonEntity
                    {
                        User = Users[1],
                        CardCollection = Collections[1],
                        Id = Guid.NewGuid(),
                        Start = DateTime.UtcNow.AddHours(-1),
                        Seen = 2,
                        Correct = 2,
                        Wrong = 0
                    });
                }
            }
        }
    }
}