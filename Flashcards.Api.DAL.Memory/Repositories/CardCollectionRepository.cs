using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using System.Linq.Expressions;
using Flashcards.Api.DAL.Common.Interfaces;

namespace Flashcards.Api.DAL.Memory.Repositories
{
    public class CardCollectionRepository : ICardCollectionRepository
    {
        private readonly IStorage storage;
        public CardCollectionRepository(IStorage storage)
        {
            this.storage = storage;
        }

        public bool Exists(Guid id)
        {
            return storage.Collections.Any(collection => collection.Id == id);
        }

        public CardCollectionEntity FirstOrDefault(Expression<Func<CardCollectionEntity, bool>> predicate)
        {
            return storage.Collections.AsQueryable().FirstOrDefault(predicate);
        }

        public IList<CardCollectionEntity> GetAll()
        {
            return storage.Collections.ToList();
        }

        public CardCollectionEntity? GetById(Guid id)
        {
            return storage.Collections.SingleOrDefault(collection => collection.Id == id);
        }

        public Guid Insert(CardCollectionEntity collection)
        {
            storage.Collections.Add(collection);
            return collection.Id;
        }

        public void Remove(Guid id)
        {
            var collectionToRemove = storage.Collections.Single(collection => collection.Id == id);
            storage.Collections.Remove(collectionToRemove);
        }

        public Guid? Update(CardCollectionEntity collection)
        {
            var existingCollection = storage.Collections.Single(collectionInStorage => collectionInStorage.Id == collection.Id);
            if (existingCollection is not null)
            {
                existingCollection.AnswerStart = collection.AnswerStart;
                existingCollection.AnswerEnd = collection.AnswerEnd;
                existingCollection.Title = collection.Title;
                existingCollection.Cards = collection.Cards;
                return existingCollection.Id;
            }

            return null;
        }

        public IEnumerable<CardCollectionEntity> Where(Expression<Func<CardCollectionEntity, bool>> predicate)
        {
            return storage.Collections.AsQueryable().Where(predicate);
        }
    }
}