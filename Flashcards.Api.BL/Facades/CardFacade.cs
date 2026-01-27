using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public class CardFacade(ICardRepository cardRepository, IUserRepository userRepository, CardMapper mapper)
        : FacadeBase<ICardRepository, CardEntity>(cardRepository, userRepository),
        ICardFacade
    {
        private readonly ICardRepository cardRepository = cardRepository;

        public Guid Create(CardCreateModel card, IList<string> userRoles, string requestorId)
        {
            var requestorUserEntity = userRepository.GetByIdentityId(requestorId);

            var cardEntity = mapper.ToEntity(card, requestorUserEntity);
            cardRepository.Insert(cardEntity);
            return cardEntity.Id;
        }

        public Guid CreateOrUpdate(CardDetailModel card, IList<string> userRoles, string requestorId)
        {
            return cardRepository.Exists(card.Id)
                ? Update(card, userRoles, requestorId)!.Value
                : Create(mapper.ToCreateModel(card), userRoles, requestorId);
        }

        public void Delete(Guid id, IList<string> userRoles, string requestorId)
        {
            cardRepository.Remove(id);
        }

        public List<CardListModel> GetAll()
        {
            var entities = cardRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public PagedResponse<CardListModel> GetByQuery(CardQueryModel query)
        {
            var rawCards = cardRepository.GetAll();
            var collection = rawCards.AsEnumerable();
            var totalCount = rawCards.Count;

            if (!string.IsNullOrEmpty(query.SearchString))
            {
                var search = query.SearchString.Trim();
                collection = collection.Where(c =>
                    (c.QuestionContent.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Description != null && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (query.QuestionType is not null)
            {
                collection = collection.Where(c => c.QuestionType == query.QuestionType);
            }

            if (query.AnswerType is not null)
            {
                collection = collection.Where(c => c.AnswerType == query.AnswerType);
            }

            if (query.OwnerId is not null)
            {
                collection = collection.Where(c => c.Owner != null && c.Owner.Id == query.OwnerId);
            }

            collection = QueryHelper.PerformPagination(collection, query);
            var result = mapper.ToListModel(collection.ToList());

            return new PagedResponse<CardListModel>(result, totalCount, query.Page, query.PageSize);
        }

        public CardDetailModel? GetById(Guid id)
        {
            var cardEntity = cardRepository.GetById(id);
            return cardEntity is null 
                ? null 
                : mapper.ToDetailModel(cardEntity);
        }

        public Guid? Update(CardDetailModel card, IList<string> userRoles, string requestorId)
        {
            // existingCard.Owner == null && isAdmin(requestedId)  => allow update
            // existingCard.Owner != null && isAdminOrOwner(requestedId, cardOwnerId) => allow update

            var cardEntity = new CardEntity()
            {
                Id = card.Id,
                QuestionType = card.QuestionType,
                AnswerType = card.AnswerType,
                QuestionContent = card.QuestionContent,
                AnswerContent = card.AnswerContent,
                Created = card.DateCreated,
                Description = card.Description
            };
            return cardRepository.Update(cardEntity);
        }
    }
}
