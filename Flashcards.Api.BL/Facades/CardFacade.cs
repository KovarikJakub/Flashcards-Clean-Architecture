using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class CardFacade : ICardFacade
    {
        private readonly ICardRepository cardRepository;
        private readonly CardMapper mapper;

        public CardFacade(ICardRepository cardRepository, CardMapper mapper)
        {
            this.cardRepository = cardRepository;
            this.mapper = mapper;
        }

        public Guid Create(CardDetailModel card)
        {
            var cardEntity = mapper.ToEntity(card);
            cardRepository.Insert(cardEntity);
            return cardEntity.Id;
        }

        public Guid CreateOrUpdate(CardDetailModel card)
        {
            return cardRepository.Exists(card.Id)
                ? Update(card)!.Value
                : Create(card);
        }

        public void Delete(Guid id)
        {
            cardRepository.Remove(id);
        }

        public List<CardListModel> GetAll()
        {
            var entities = cardRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public List<CardListModel> GetByQuery(CardQueryModel query)
        {
            var collection = cardRepository.GetAll().AsEnumerable();
            
            if (!string.IsNullOrEmpty(query.SearchString))
            {
                var search = query.SearchString.Trim();
                collection = collection.Where(c =>
                    (c.QuestionContent.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Description != null && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                );
            }

            collection = QueryHelper.PerformPagination(collection, query);
            
            return mapper.ToListModel(collection.ToList());
        }

        public CardDetailModel? GetById(Guid id)
        {
            var cardEntity = cardRepository.GetById(id);
            return cardEntity is null 
                ? null 
                : mapper.ToDetailModel(cardEntity);
        }

        public Guid? Update(CardDetailModel card)
        {
            var cardEntity = mapper.ToEntity(card);
            return cardRepository.Update(cardEntity);
        }
    }
}
