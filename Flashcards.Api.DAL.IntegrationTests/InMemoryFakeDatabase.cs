
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Api.DAL.Memory;
using Flashcards.Api.DAL.Memory.Repositories;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.IntegrationTests
{
    public class InMemoryFakeDatabase : IFakeDatabase
    {
        private readonly Storage inMemoryStorage;

        public InMemoryFakeDatabase()
        {
            inMemoryStorage = new Storage(createWithDummyData: false);
            SeedStorage();
        }

        public ICardRepository GetCardRepository()
        {
            return new CardRepository(inMemoryStorage);
        }
        
        public CardEntity GetCardById(Guid id)
        {
            return inMemoryStorage.Cards.Single(card => card.Id == id);
        }
        
        public IList<Guid> CardGuids { get; } = new List<Guid>
        {
            new("3b07c916-a86e-4992-a58b-0158d3232335"),
            new("3807e412-bf2b-4825-b361-039215cb4eaa"),
            new("803a24c3-2c93-4fc8-b96d-4c8f5ee63ecf")
        };
        
        public IUserRepository GetUserRepository()
        {
            return new UserRepository(inMemoryStorage);
        }

        public UserEntity? GetUserById(Guid id)
        {
            return inMemoryStorage.Users.SingleOrDefault(user => user.Id == id);
        }

        public IList<UserEntity> GetAll()
        {
            return inMemoryStorage.Users.ToList();
        }
        
        public IList<Guid> UserGuids { get; } = new List<Guid>
        {
            new("f98b43b3-2c65-4e84-9538-094e7dd08a3d"),
            new("6a2cabfb-b818-4fac-a6b3-1b865536d8a6"),
            new("bd4422e9-f4b3-43d7-a8ce-165aa8a021ff"),
            new("5b6574e9-037b-422e-8c5c-f421e4f2c82b")
        };

        public ICardCollectionRepository GetCardCollectionRepository()
        {
            return new CardCollectionRepository(inMemoryStorage);
        }

        public CardCollectionEntity GetCardCollectionById(Guid id)
        {
            return inMemoryStorage.Collections.SingleOrDefault(collection => collection.Id == id);
        }

        public IList<Guid> CardCollectionGuids { get; } = new List<Guid>
        {
            new("a6b56d25-89e0-41f5-b785-5c1a44a5c54d"),
            new("e1f4b0f7-9db1-4877-812f-1cc1e4fbc7d8"),
            new("c74d2e0c-5989-4f6b-a83c-fb0a3c514f6d")

        };

        private void SeedStorage()
        {
            inMemoryStorage.Cards.Add(new CardEntity
            {
                Id = CardGuids[0],
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "Who is showing up to friday morning ISA lecture?",
                AnswerContent = "Not me",
                DateCreated = new DateTime(2025, 2, 1, 10,0,0,0),
                Description = "A common sense question."
            });

            inMemoryStorage.Cards.Add(new CardEntity
            {
                Id = CardGuids[1],
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "Best asian food?",
                AnswerContent = "Kottu",
                DateCreated = new DateTime(2025, 2, 2, 11, 0, 0, 0),
                Description = "A general question"
            });

            inMemoryStorage.Cards.Add(new CardEntity
            {
                Id = CardGuids[2],
                QuestionType = ContentType.Text,
                AnswerType = ContentType.Text,
                QuestionContent = "What is the most overhyped programming language?",
                AnswerContent = "Rust",
                DateCreated = new DateTime(2025, 2, 3, 9, 0, 0, 0),
                Description = "A programming question"
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[0],
                Name = "Pavel",
                PhotographyUrl = null,
                Role = RoleType.User
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[1],
                Name = "Oliver",
                PhotographyUrl = null,
                Role = RoleType.User
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[2],
                Name = "Vit",
                PhotographyUrl = null,
                Role = RoleType.User
            });
            
            inMemoryStorage.Users.Add(new UserEntity
            {
                Id = UserGuids[3],
                Name = "Adam",
                PhotographyUrl = null,
                Role = RoleType.Administrator
            });

            inMemoryStorage.Collections.Add(new CardCollectionEntity
            {
                Id = CardCollectionGuids[0],
                Title = "General Knowledge",
                AnswerStart = new DateTime(2025, 2, 10, 8, 0, 0, 0),
                AnswerEnd = new DateTime(2025, 2, 10, 10, 0, 0, 0),
                Cards = new List<CardEntity> { inMemoryStorage.Cards[0], inMemoryStorage.Cards[1] }
            });

            inMemoryStorage.Collections.Add(new CardCollectionEntity
            {
                Id = CardCollectionGuids[1],
                Title = "Programming Trivia",
                AnswerStart = new DateTime(2025, 2, 11, 9, 0, 0, 0),
                AnswerEnd = new DateTime(2025, 2, 11, 11, 0, 0, 0),
                Cards = new List<CardEntity> { inMemoryStorage.Cards[2] }
            });

            inMemoryStorage.Collections.Add(new CardCollectionEntity
            {
                Id = CardCollectionGuids[2],
                Title = "Random Fun Facts",
                AnswerStart = new DateTime(2025, 2, 12, 8, 30, 0, 0),
                AnswerEnd = new DateTime(2025, 2, 12, 9, 30, 0, 0),
                Cards = new List<CardEntity> { inMemoryStorage.Cards[1] }
            });
        }
    }
}
