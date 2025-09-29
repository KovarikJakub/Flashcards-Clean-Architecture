
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
        }
    }
}
