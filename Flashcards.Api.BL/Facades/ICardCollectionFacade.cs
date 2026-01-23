using Flashcards.Common.BL.Facades;
using Flashcards.Common.Models;

namespace Flashcards.Api.BL.Facades
{
    public interface ICardCollectionFacade : IFacade
    {
        List<CardCollectionListModel> GetAll();
        List<CardCollectionListModel> GetByQuery(CardCollectionQueryModel query);
        CardCollectionDetailModel? GetById(Guid id);
        List<CardCollectionListModel> GetByOwner(string ownerId);
        Guid CreateOrUpdate(CardCollectionDetailModel CardCollectionModel, IList<string> userRoles, string? ownerId = null);
        Guid Create(CardCollectionDetailModel CardCollectionModel, IList<string> userRoles, string? ownerId = null);
        Guid? Update(CardCollectionDetailModel CardCollectionModel, IList<string> userRoles, string? ownerId = null);
        void Delete(Guid id, IList<string> userRoles, string? ownerId = null);
    }
}
