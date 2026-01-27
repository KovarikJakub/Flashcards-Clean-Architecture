using Flashcards.Api.BL.Mappers;
using Flashcards.Api.DAL.Common.Entities;
using Flashcards.Api.DAL.Common.Repositories;
using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using System;

namespace Flashcards.Api.BL.Facades
{
    public class CardCollectionFacade(
        ICardCollectionRepository cardCollectionRepository,
        IUserRepository userRepository,
        CardCollectionMapper mapper)
        : FacadeBase<ICardCollectionRepository, CardCollectionEntity>(cardCollectionRepository, userRepository),
        ICardCollectionFacade
    {
        public List<CardCollectionListModel> GetAll()
        {
            var entities = cardCollectionRepository.GetAll();
            return mapper.ToListModel(entities);
        }

        public PagedResponse<CardCollectionListModel> GetByQuery(CardCollectionQueryModel query)
        {
            var rawCollections = cardCollectionRepository.GetAll();
            var collection = rawCollections.AsEnumerable();
            var totalCount = rawCollections.Count;

            if (!string.IsNullOrEmpty(query.SearchString))
            {
                collection = collection.Where(c =>
                    c.Title.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase));
            }

            if (query.OwnerId is not null)
            {
                collection = collection.Where(c => c.OwnerId == query.OwnerId);
            }

            if (query.ExcludeOwnerId is not null)
            {
                collection = collection.Where(c => c.OwnerId != query.ExcludeOwnerId);
            }

            if (query.ShowCurrentlyAvailableOnly)
            {
                var now = DateTime.Now;
                collection = collection.Where(c => c.AnswerStart <= now && now <= c.AnswerEnd);
            }

            collection = QueryHelper.PerformPagination(collection, query);
            var result = mapper.ToListModel(collection.ToList());

            return new PagedResponse<CardCollectionListModel>(result, totalCount, query.Page, query.PageSize);
        }

        public CardCollectionDetailModel? GetById(Guid id)
        {
            var entity = cardCollectionRepository.GetById(id);
            return entity == null ? null : mapper.ToDetailModel(entity);
        }

        public Guid CreateOrUpdate(CardCollectionDetailModel model, IList<string> userRoles, string requestorId)
        {
            return cardCollectionRepository.Exists(model.Id)
                ? Update(model, userRoles, requestorId)!.Value
                : Create(mapper.ToCreateModel(model), userRoles, requestorId);
        }

        public Guid Create(CardCollectionCreateModel model, IList<string> userRoles, string requestorId)
        {
            var requestorUserEntity = userRepository.GetByIdentityId(requestorId);

            var entity = mapper.ToEntity(model, requestorUserEntity);
            cardCollectionRepository.Insert(entity);
            return entity.Id;
        }

        public Guid? Update(CardCollectionDetailModel model, IList<string> userRoles, string requestorId)
        {
            var existing = cardCollectionRepository.GetById(model.Id);
            if (existing == null)
                return null;
            var ownerEntity = userRepository.GetById(existing.OwnerId);
            var entity = mapper.ToEntity(model, ownerEntity!);
            entity.OwnerId = existing.OwnerId;
            return cardCollectionRepository.Update(entity);
        }

        public void Delete(Guid id, IList<string> userRoles, string requestorId)
        {
            cardCollectionRepository.Remove(id);
        }
    }
}
