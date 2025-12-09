using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class CardCollectionFacade : ICardCollectionFacade
    {
        private readonly ICardCollectionRepository cardCollectionRepository;
        private readonly CardCollectionMapper mapper;

        public CardCollectionFacade(
            ICardCollectionRepository cardCollectionRepository,
            CardCollectionMapper mapper)
        {
            this.cardCollectionRepository = cardCollectionRepository;
            this.mapper = mapper;
        }

        public List<CardCollectionListModel> GetAll()
        {
            var entities = cardCollectionRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public CardCollectionDetailModel? GetById(Guid id)
        {
            var entity = cardCollectionRepository.GetById(id);
            return entity == null ? null : mapper.ToDetailModel(entity);
        }

        public Guid CreateOrUpdate(CardCollectionDetailModel model, IList<string> userRoles, string? ownerId = null)
        {
            ThrowIfWrongOwnerAndNotAdmin(model.Id, userRoles, ownerId);
            return cardCollectionRepository.Exists(model.Id)
                ? Update(model, userRoles, ownerId)!.Value
                : Create(model, userRoles, ownerId);
        }

        public Guid Create(CardCollectionDetailModel model, IList<string> userRoles, string? ownerId = null)
        {
            model.OwnerId = ownerId;
            var entity = mapper.ToEntity(model);
            cardCollectionRepository.Insert(entity);
            return entity.Id;
        }

        public Guid? Update(CardCollectionDetailModel model, IList<string> userRoles, string? ownerId = null)
        {
            ThrowIfWrongOwnerAndNotAdmin(model.Id, userRoles, ownerId);

            var existing = cardCollectionRepository.GetById(model.Id);
            if (existing == null)
                return null;

            var entity = mapper.ToEntity(model);
            entity.OwnerId = existing.OwnerId;

            entity.Cards = model.Cards.Select(c =>
                new CardEntity
                {
                    Id = c.Id,
                    QuestionType = c.QuestionType,
                    QuestionContent = c.QuestionContent,
                    DateCreated = c.DateCreated,
                    AnswerType = c.AnswerType,
                    AnswerContent = c.AnswerContent,
                    Description = c.Description
                }).ToList();

            return cardCollectionRepository.Update(entity);
        }

        public void Delete(Guid id, IList<string> userRoles, string? ownerId = null)
        {
            ThrowIfWrongOwnerAndNotAdmin(id, userRoles, ownerId);
            cardCollectionRepository.Remove(id);
        }

        private void ThrowIfWrongOwnerAndNotAdmin(Guid? id, IList<string> userRoles, string? ownerId)
        {
            if (id is not null
                && userRoles is not null
                && !userRoles.Contains(RoleType.Administrator.ToString())
                && ownerId is not null
                && cardCollectionRepository.GetById(id.Value)?.OwnerId != ownerId)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}
