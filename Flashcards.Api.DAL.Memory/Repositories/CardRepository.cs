using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using System.Linq.Expressions;

namespace Flashcards.Api.DAL.Memory.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly Storage storage;
        public CardRepository(Storage storage) 
        {
            this.storage = storage;
        }

        public bool Exists(Guid id)
        {
            return storage.Cards.Any(card => card.Id == id);
        }

        public CardEntity FirstOrDefault(Expression<Func<CardEntity, bool>> predicate)
        {
            return storage.Cards.AsQueryable().FirstOrDefault(predicate);
        }

        public IList<CardEntity> GetAll()
        {
            return storage.Cards.ToList();
        }

        public CardEntity? GetById(Guid id)
        {
            return storage.Cards.SingleOrDefault(card => card.Id == id);
        }

        public Guid Insert(CardEntity card)
        {
            storage.Cards.Add(card);
            return card.Id;
        }

        public void Remove(Guid id)
        {
            var cardToRemove = storage.Cards.Single(card => card.Id == id);
            storage.Cards.Remove(cardToRemove);
        }

        public Guid? Update(CardEntity card)
        {
            var existingCard = storage.Cards.Single(cardInStorage => cardInStorage.Id == card.Id);
            if(existingCard is not null)
            {
                existingCard.AnswerContent = card.AnswerContent;
                existingCard.QuestionContent = card.QuestionContent;
                existingCard.DateCreated = card.DateCreated;
                existingCard.AnswerType = card.AnswerType;
                existingCard.QuestionType = card.QuestionType;
                return existingCard.Id;
            }

            return null;
        }

        public IEnumerable<CardEntity> Where(Expression<Func<CardEntity, bool>> predicate)
        {
            return storage.Cards.AsQueryable().Where(predicate);
        }
    }
}
