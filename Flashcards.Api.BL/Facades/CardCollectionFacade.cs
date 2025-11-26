using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class CardCollectionFacade : ICardCollectionFacade
    {
        private readonly ICardCollectionRepository cardCollectionRepository;
        private readonly CardCollectionMapper mapper;

        public CardCollectionFacade(
            ICardCollectionRepository CardCollectionRepository,
            CardCollectionMapper mapper)
        {
            this.cardCollectionRepository = CardCollectionRepository;
            this.mapper = mapper;
        }

        public List<CardCollectionListModel> GetAll()
        {
            var CardCollectionEntities = cardCollectionRepository.GetAll();
            return mapper.ToListModel(CardCollectionEntities);
        }

        public CardCollectionDetailModel? GetById(Guid id)
        {
            var CardCollectionEntity = cardCollectionRepository.GetById(id);
            return CardCollectionEntity == null
                ? null
                : mapper.ToDetailModel(CardCollectionEntity);
        }

        public Guid CreateOrUpdate(CardCollectionDetailModel CardCollectionModel)
        {
            return cardCollectionRepository.Exists(CardCollectionModel.Id)
                ? Update(CardCollectionModel)!.Value
                : Create(CardCollectionModel);
        }

        public Guid Create(CardCollectionDetailModel CardCollectionModel)
        {
           
            var CardCollectionEntity = mapper.ToEntity(CardCollectionModel);
            cardCollectionRepository.Insert(CardCollectionEntity);
            return CardCollectionEntity.Id;
        }

        public Guid? Update(CardCollectionDetailModel cardCollectionModel)
        {
            

            var cardCollectionEntity = mapper.ToEntity(cardCollectionModel);
            cardCollectionEntity.Cards = cardCollectionModel.Cards.Select(c =>
                new CardEntity
                {
                    Id = c.Id,
                    QuestionType = c.QuestionType,
                    QuestionContent = c.QuestionContent,
                    DateCreated = c.DateCreated,
                    AnswerType = c.AnswerType,
                    AnswerContent = c.AnswerContent                   
                }).ToList();
            var result = cardCollectionRepository.Update(cardCollectionEntity);
            return result;
        }

       

        public void Delete(Guid id)
        {
            cardCollectionRepository.Remove(id);
        }
    }
}