
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Common.Enums;

namespace Flashcards.Api.DAL.Memory
{
    public class Storage
    {
        public IList<CardEntity> Cards { get; } = new List<CardEntity>();

        public Storage(bool createWithDummyData = true) 
        {
            if (createWithDummyData)
            {
                SeedCards();
                // TODO: Seed other entities here
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
    }
}
